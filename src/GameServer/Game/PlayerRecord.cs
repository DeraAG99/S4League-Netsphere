using System;
using System.IO;
using System.Linq;
using NeoNetsphere;
using NeoNetsphere.Network;
using NeoNetsphere.Resource;

// ReSharper disable once Checknamespace
namespace NeoNetsphere.Game
{
  internal abstract class PlayerRecord
  {
    protected PlayerRecord(Player player)
    {
      Player = player;
      Player.RoomInfo.Stats = this;
    }

    public Player Player { get; }
    public abstract uint TotalScore { get; }
    public uint Kills { get; set; }
    public uint KillAssists { get; set; }
    public uint Suicides { get; set; }
    public uint Deaths { get; set; }

    public virtual uint GetPenGain(out uint bonusPen)
    {
      bonusPen = 0;
      var pointBonus = GameServer.Instance.ResourceCache.GetPointBonus();
      var plrLevel = Player.Level;

      PointBonusEntry penConfig = null;
      switch (Player.Room.GameRuleManager.GameRule.GameRule)
      {
        case GameRule.Touchdown:
        case GameRule.PassTouchdown:
        case GameRule.SemiTouchdown:
          penConfig = pointBonus.Touchdown;
          break;
        case GameRule.Deathmatch:
          penConfig = pointBonus.Deathmatch;
          break;
        case GameRule.Survival:
          penConfig = pointBonus.Survival;
          break;
        case GameRule.Captain:
          penConfig = pointBonus.Captain;
          break;
        case GameRule.Chaser:
          penConfig = pointBonus.Chaser;
          break;
        case GameRule.BattleRoyal:
          penConfig = pointBonus.BattleRoyal;
          break;
        case GameRule.SnowballFight:
          penConfig = pointBonus.SnowballFight;
          break;
        case GameRule.Arcade:
          penConfig = pointBonus.Arcade;
          break;
        case GameRule.Horde:
          penConfig = pointBonus.Horde;
          break;
        case GameRule.Siege:
          penConfig = pointBonus.Siege;
          break;
      }

      if (penConfig == null || !penConfig.IsValid)
      {
        // Fallback: derive PEN from EXP
        var exp = GetExpGain(out var _);
        return (uint)Math.Max(0, exp);
      }

      var plrs = Player.Room.TeamManager.Players
          .Where(plr => plr.RoomInfo.State == PlayerState.Waiting &&
                        plr.RoomInfo.Mode == PlayerGameMode.Normal)
          .ToArray();

      var place = 1;
      foreach (var plr in plrs.OrderByDescending(plr => plr.RoomInfo.Stats.TotalScore))
      {
        if (plr == Player)
          break;
        place++;
        if (place > 3)
          break;
      }

      var rankingBonus = 0f;
      switch (place)
      {
        case 1:
          rankingBonus = 1.0f;
          break;
        case 2:
          rankingBonus = 0.7f;
          break;
        case 3:
          rankingBonus = 0.4f;
          break;
      }

      var pen = TotalScore * penConfig.RankingFactor +
                plrs.Length * penConfig.PlayerCountFactor +
                Player.RoomInfo.PlayTime.TotalMinutes * penConfig.PointPerMin;

      // Apply ranking bonus
      pen *= (1.0f + rankingBonus);

      // Apply level bonus
      foreach (var levelBonus in pointBonus.LevelBonuses)
      {
        if (plrLevel >= levelBonus.Min && plrLevel <= levelBonus.Max)
        {
          pen += levelBonus.PenBonus;
          break;
        }
      }

      return (uint)Math.Max(1, pen);
    }

    public virtual int GetExpGain(out int bonusExp)
    {
      bonusExp = 0;
      var expBonus = GameServer.Instance.ResourceCache.GetExperienceBonus();

      ExperienceBonusEntry expConfig = null;
      switch (Player.Room.GameRuleManager.GameRule.GameRule)
      {
        case GameRule.Touchdown:
        case GameRule.PassTouchdown:
        case GameRule.SemiTouchdown:
          expConfig = expBonus.Touchdown;
          break;
        case GameRule.Deathmatch:
          expConfig = expBonus.Deathmatch;
          break;
        case GameRule.Survival:
          expConfig = expBonus.Survival;
          break;
        case GameRule.Captain:
          expConfig = expBonus.Captain;
          break;
        case GameRule.Chaser:
          expConfig = expBonus.Chaser;
          break;
        case GameRule.BattleRoyal:
          expConfig = expBonus.BattleRoyal;
          break;
        case GameRule.SnowballFight:
          expConfig = expBonus.SnowballFight;
          break;
        case GameRule.Horde:
          expConfig = expBonus.Horde;
          break;
        case GameRule.Siege:
          expConfig = expBonus.Seize;
          break;
      }

      if (expConfig == null || !expConfig.IsValid)
        return 0;

      var plrs = Player.Room.TeamManager.Players
          .Where(plr => plr.RoomInfo.State == PlayerState.Waiting &&
                        plr.RoomInfo.Mode == PlayerGameMode.Normal)
          .ToArray();

      var place = 1;
      foreach (var plr in plrs.OrderByDescending(plr => plr.RoomInfo.Stats.TotalScore))
      {
        if (plr == Player)
          break;
        place++;
        if (place > 3)
          break;
      }

      var rankingBonus = 1.0f;
      switch (place)
      {
        case 1:
          rankingBonus += expConfig.RankingFactor;
          break;
        case 2:
          rankingBonus += expConfig.RankingFactor * 0.6f;
          break;
        case 3:
          rankingBonus += expConfig.RankingFactor * 0.3f;
          break;
      }

      var timeExp = expConfig.ConstantExpPerMin * Player.RoomInfo.PlayTime.TotalMinutes;
      var variableExp = expConfig.VariableExpPerMin * Player.RoomInfo.PlayTime.TotalMinutes;
      var playersExp = plrs.Length * expConfig.PlayerCountFactor;
      var scoreExp = variableExp > 0 ? TotalScore * (variableExp / 100.0f) : 0;

      var expGained = (timeExp + playersExp + scoreExp) * rankingBonus;

      bonusExp = (int)expGained;
      return (int)Math.Round(expGained);
    }

    public virtual void Reset()
    {
      Kills = 0;
      KillAssists = 0;
      Suicides = 0;
      Deaths = 0;
      Player.LuckyShot.Clear();
    }

    public virtual void Serialize(BinaryWriter w, bool isResult)
    {
      if (Player?.Account == null)
        return;

      if (Player?.Account?.Id == 0)
        return;

      if (Player.RoomInfo.Team == null)
        return;

      w.Write(Player.Account.Id); // Int64
      w.Write((byte)Player.RoomInfo.Team.Team); // Int8
      w.Write((byte)Player.RoomInfo.State); // Int8
      w.Write(Convert.ToByte(Player.RoomInfo.IsReady)); // Int8
      w.Write((uint)Player.RoomInfo.Mode); // Int32
      w.Write(TotalScore); // Int32
      w.Write(0); // Int32

      uint bonusPen = 0;
      var bonusExp = 0;
      var rankUp = false;
      if (isResult && Player.RoomInfo.State != PlayerState.Lobby)
      {
        var penGain = GetPenGain(out bonusPen);
        var expGain = GetExpGain(out bonusExp);
        if (Player.Room.Options.IsFriendly)
        {
          expGain = 0;
          penGain /= 80;

          bonusExp = 0;
          bonusPen /= 80;
        }

        penGain += (uint)Player.LuckyShot.BonusPen;
        expGain += Player.LuckyShot.BonusExp;

        bonusPen += (uint)Player.LuckyShot.BonusPen;
        bonusExp += Player.LuckyShot.BonusExp;
        w.Write(penGain); // Int32
        w.Write(expGain); // Int32
        Player.PEN += (penGain + bonusPen);
        rankUp = Player.GainExp(expGain + bonusExp);
      }
      else
      {
        w.Write(0);
        w.Write(0);
      }

      w.Write(Player.TotalExperience); // Int32
      w.Write(rankUp); // Int8
      w.Write(bonusExp); // Int32
      w.Write(bonusPen); // Int32
      w.Write(0); // Int32

      /*
          1 PC Room(korean internet cafe event)
          2 PEN+
          4 EXP+
          8 20%
          16 25%
          32 30%
      */

      w.Write(0); // Int32
      w.Write((byte)0); // Int8
      w.Write((byte)0); // Int8
      w.Write((byte)0); // Int8
      w.Write(0); // Int32
      w.Write(0); // Int32
      w.Write(0); // Int32
      w.Write(0); // Int32

      // NEW - UNKNOWN
      w.Write(0); // Int32
      w.Write((byte)0); // Int8 -- player room index?? team?
      w.Write(0); // Int32
      w.Write(0); // Int32
    }
  }
}
