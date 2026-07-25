using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using BlubLib.Configuration;
using NeoNetsphere.Network;
using NeoNetsphere.Resource.xml;
using Serilog;
using Serilog.Core;

namespace NeoNetsphere.Resource
{
  internal class ResourceLoader
  {
    // ReSharper disable once InconsistentNaming
    private static readonly ILogger Logger =
        Log.ForContext(Constants.SourceContextPropertyName, nameof(ResourceLoader));

    public ResourceLoader(string resourcePath)
    {
      ResourcePath = resourcePath;
    }

    public string ResourcePath { get; }

    public byte[] GetBytes(string fileName)
    {
      var path = Path.Combine(ResourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
      return File.Exists(path) ? File.ReadAllBytes(path) : null;
    }

    public IEnumerable<Experience> LoadExperience()
    {
      var dto = Deserialize<ExperienceDto>("xml/experience.x7");

      var i = 0;
      return dto.exp.Select(expDto => new Experience
      {
        Level = i++,
        ExperienceToNextLevel = expDto.require,
        TotalExperience = expDto.accumulate
      });
    }

    public IEnumerable<MapInfo> LoadMaps()
    {
      var stringTable = Deserialize<StringTableDto>("language/xml/gameinfo_string_table.x7");
      var dto = Deserialize<MapInfoDto>("xml/map.x7");

      var maps = new ConcurrentDictionary<Tuple<GameRule, byte>, MapInfo>();

      foreach (var mapDto in dto.map)
      {
        var pf = mapDto.resource?.previewinfo_path ?? "";
        if (!pf.EndsWith(".tga") &&
            !pf.EndsWith(".dds"))
          continue;

        var seu = mapDto.Switch?.eu ?? "";
        var skr = mapDto.Switch?.kr ?? "";
        if (seu != "on" &&
            skr != "on")
          continue;

        var byteId = unchecked((byte)mapDto.id);

        var map = new MapInfo
        {
          Id = mapDto.id,
          byteId = byteId,
          MinLevel = 0,
          ServerId = 0,
          ChannelId = 0,
          RespawnType = 0,
          MaxPlayers = mapDto.Base.limit_player,
          IsRandom = mapDto.id > 900,
          GameRule = (GameRule)mapDto.Base.mode_number
        };

        var name_ = new StringTableStringDto();
        try
        {
          name_ = stringTable.@string.First(s =>
              s.key.Equals(mapDto.Base.map_name_key, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception ex)
        {
          name_.eng = "unknown";
        }

        var name = name_;
        if (string.IsNullOrWhiteSpace(name.eng))
          name.eng = mapDto.Base.map_name_key;

        map.Name = name.eng;
        maps.TryAdd(new Tuple<GameRule, byte>(map.GameRule, byteId), map);
      }

      return maps.Values;
    }

    public IEnumerable<ItemEffect> LoadEffects()
    {
      var dto = Deserialize<ItemEffectDto>("xml/item_effect.x7");
      var stringTable = Deserialize<StringTableDto>("language/xml/item_effect_string_table.x7");

      foreach (var itemEffectDto in dto.item.Where(itemEffect => itemEffect.id != 0))
      {
        var itemEffect = new ItemEffect
        {
          Id = itemEffectDto.id
        };

        foreach (var attributeDto in itemEffectDto.attribute)
        {
          itemEffect.Attributes.Add(new ItemEffectAttribute
          {
            Attribute = (Attribute)Enum.Parse(typeof(Attribute), attributeDto.effect.Replace("_", ""),
                  true),
            Value = attributeDto.value,
            Rate = float.Parse(attributeDto.rate, CultureInfo.InvariantCulture)
          });
        }

        var name = stringTable.@string.FirstOrDefault(s =>
            s.key.Equals(itemEffectDto.text_key, StringComparison.InvariantCultureIgnoreCase));

        if (name == null)
          name = new StringTableStringDto();

        if (string.IsNullOrWhiteSpace(name.eng))
          name.eng = itemEffectDto.NAME;

        itemEffect.Name = name.eng;
        yield return itemEffect;
      }
    }

    public IEnumerable<GameTempo> LoadGameTempos()
    {
      var dto = Deserialize<ConstantInfoDto>("xml/constant_info.x7");

      foreach (var gameTempoDto in dto.GAMEINFOLIST)
      {
        var tempo = new GameTempo
        {
          Name = gameTempoDto.TEMPVALUE.value
        };

        var values = gameTempoDto.GAMETEPMO_COMMON_TOTAL_VALUE;
        tempo.ActorDefaultHPMax =
            float.Parse(values.GAMETEMPO_actor_default_hp_max, CultureInfo.InvariantCulture);
        tempo.ActorDefaultMPMax =
            float.Parse(values.GAMETEMPO_actor_default_mp_max, CultureInfo.InvariantCulture);
        tempo.ActorDefaultMoveSpeed = values.GAMETEMPO_fastrun_required_mp;

        yield return tempo;
      }
    }

    #region DefaultItems

    public IEnumerable<DefaultItem> LoadDefaultItems()
    {
      var dto = Deserialize<DefaultItemDto>("xml/default_item.x7");

      foreach (var itemDto in dto.male.item)
      {
        var item = new DefaultItem
        {
          ItemNumber = new ItemNumber(itemDto.category, itemDto.sub_category, itemDto.number),
          Gender = CharacterGender.Male,
          //Slot = (byte) ParseDefaultItemSlot(itemDto.Value),
          Variation = itemDto.variation
        };
        yield return item;
      }

      foreach (var itemDto in dto.female.item)
      {
        var item = new DefaultItem
        {
          ItemNumber = new ItemNumber(itemDto.category, itemDto.sub_category, itemDto.number),
          Gender = CharacterGender.Female,
          //Slot = (byte) ParseDefaultItemSlot(itemDto.Value),
          Variation = itemDto.variation
        };
        yield return item;
      }
    }

    #endregion

    private static readonly Regex XmlCommentRegex = new Regex(@"<!--[\s\S]*?-->", RegexOptions.Compiled);
    private static readonly Regex MissingSpaceRegex = new Regex(@"(""[^""]*"")([a-zA-Z_])", RegexOptions.Compiled);

    private T Deserialize<T>(string fileName)
    {
      var serializer = new XmlSerializer(typeof(T));

      var path = Path.Combine(ResourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
      using (var r = new StreamReader(path))
      {
        try
        {
          return (T)serializer.Deserialize(r);
        }
        catch (InvalidOperationException)
        {
        }
      }

      var content = File.ReadAllText(path);
      content = XmlCommentRegex.Replace(content, "");
      content = MissingSpaceRegex.Replace(content, "$1 $2");
      using (var r = new StringReader(content))
      {
        return (T)serializer.Deserialize(r);
      }
    }

    #region Items

    public IEnumerable<ItemInfo> LoadItems()
    {
      var dto = Deserialize<ItemInfoDto>("xml/iteminfo.x7");
      var stringTable = Deserialize<StringTableDto>("language/xml/iteminfo_string_table.xml");

      foreach (var categoryDto in dto.category)
      {
        foreach (var subCategoryDto in categoryDto.sub_category)
        {
          foreach (var itemDto in subCategoryDto.item)
          {
            var id = new ItemNumber(categoryDto.id, subCategoryDto.id, itemDto.number);
            ItemInfo item;

            switch (id.Category)
            {
              case ItemCategory.Skill:
                item = LoadAction(id, itemDto);
                break;

              case ItemCategory.Weapon:
                item = LoadWeapon(id, itemDto);
                break;

              default:
                item = new ItemInfo();
                break;
            }

            item.ItemNumber = id;
            item.Level = itemDto.@base.base_info.require_level;
            item.MasterLevel = itemDto.@base.base_info.require_master;
            item.Gender = ParseGender(itemDto.SEX);
            item.Image = itemDto.client.icon.image;

            if (itemDto.@base.license != null)
              item.License = ParseItemLicense(itemDto.@base.license.require);

            var name = stringTable.@string.FirstOrDefault(s =>
                s.key.Equals(itemDto.@base.base_info.name_key,
                    StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(name?.eng))
              item.Name = name != null ? name.key : itemDto.NAME;
            else
              item.Name = name.eng;

            yield return item;
          }
        }
      }
    }

    public IEnumerable<ItemInfo> LoadItems_2()
    {
      var dto = Deserialize<ItemInfoDto_2>("xml/item.x7");
      var stringTable = Deserialize<StringTableDto_2>("language/xml/iteminfo_string_table.x7");
      var ids = new List<ItemNumber>();
      foreach (var itemDto in dto.item)
      {
        var id = new ItemNumber(itemDto.item_key);
        if (!ids.Contains(id))
        {
          ids.Add(id);
          var item = new ItemInfo();
          item.ItemNumber = id;
          item.Level = 0;
          item.MasterLevel = 0;
          item.Gender = ParseGender_2(itemDto.Base.sex);
          item.Image = itemDto.graphic.icon_image;

          var name = stringTable.@string.FirstOrDefault(s =>
              s.key.Equals(itemDto.Base.name_key, StringComparison.InvariantCultureIgnoreCase));
          if (!string.IsNullOrWhiteSpace(name?.eng) && name?.eng.ToLower() != "no trans" &&
              name?.eng.ToLower() != "not trans")
            yield return item;
        }
      }
    }

    public IEnumerable<ItemInfo> LoadItems_3()
    {
      var dto = Deserialize<ItemInfoDto_2>("xml/item.x7");
      var dto2 = Deserialize<ItemInfoDto_3>("xml/dumpeditems.xml");
      var stringTable = Deserialize<StringTableDto_2>("language/xml/iteminfo_string_table.x7");
      var ids = new Dictionary<ItemNumber, ItemInfo>();
      foreach (var itemDto in dto.item)
      {
        var id = new ItemNumber(itemDto.item_key);
        if (!ids.Keys.Contains(id))
        {
          var item = new ItemInfo();
          item.ItemNumber = id;
          item.Level = 0;
          item.MasterLevel = 0;
          item.Gender = ParseGender_2(itemDto.Base.sex);
          item.Image = itemDto.graphic.icon_image;
          ids.Add(id, item);
        }
      }

      foreach (var itemdto in dto2.Item)
      {
        ItemInfo item;
        ids.TryGetValue(new ItemNumber(itemdto.ID), out item);
        if (item != null)
        {
          item.Colors = (int)itemdto.Color_Count;
          item.Name = itemdto.Name;

          if (!string.IsNullOrWhiteSpace(item.Name) &&
              item.Name != "not trans" &&
              item.Name != "no trans" &&
              !string.IsNullOrWhiteSpace(item.Image))
            yield return item;
        }
      }
    }

    private static ItemLicense ParseItemLicense(string license)
    {
      Func<string, bool> equals = str => license.Equals(str, StringComparison.InvariantCultureIgnoreCase);

      if (equals("license_none"))
        return ItemLicense.None;

      if (equals("LICENSE_CHECK_NONE"))
        return ItemLicense.None;

      if (equals("LICENSE_PLASMA_SWORD"))
        return ItemLicense.PlasmaSword;

      if (equals("license_counter_sword"))
        return ItemLicense.CounterSword;

      if (equals("LICENSE_STORM_BAT"))
        return ItemLicense.StormBat;

      if (equals("LICENSE_ASSASSIN_CLAW"))
        return ItemLicense.None; // ToDo

      if (equals("LICENSE_SUBMACHINE_GUN"))
        return ItemLicense.SubmachineGun;

      if (equals("license_revolver"))
        return ItemLicense.Revolver;

      if (equals("license_semi_rifle"))
        return ItemLicense.SemiRifle;

      if (equals("LICENSE_SMG3"))
        return ItemLicense.None; // ToDo

      if (equals("license_HAND_GUN"))
        return ItemLicense.None; // ToDo

      if (equals("LICENSE_SMG4"))
        return ItemLicense.None; // ToDo

      if (equals("LICENSE_HEAVYMACHINE_GUN"))
        return ItemLicense.HeavymachineGun;

      if (equals("LICENSE_GAUSS_RIFLE"))
        return ItemLicense.GaussRifle;

      if (equals("license_rail_gun"))
        return ItemLicense.RailGun;

      if (equals("license_cannonade"))
        return ItemLicense.Cannonade;

      if (equals("LICENSE_CENTRYGUN"))
        return ItemLicense.Sentrygun;

      if (equals("license_centi_force"))
        return ItemLicense.SentiForce;

      if (equals("LICENSE_SENTINEL"))
        return ItemLicense.SentiNel;

      if (equals("license_mine_gun"))
        return ItemLicense.MineGun;

      if (equals("LICENSE_MIND_ENERGY"))
        return ItemLicense.MindEnergy;

      if (equals("license_mind_shock"))
        return ItemLicense.MindShock;

      // SKILLS

      if (equals("LICENSE_ANCHORING"))
        return ItemLicense.Anchoring;

      if (equals("LICENSE_FLYING"))
        return ItemLicense.Flying;

      if (equals("LICENSE_INVISIBLE"))
        return ItemLicense.Invisible;

      if (equals("license_detect"))
        return ItemLicense.Detect;

      if (equals("LICENSE_SHIELD"))
        return ItemLicense.Shield;

      if (equals("LICENSE_BLOCK"))
        return ItemLicense.Block;

      if (equals("LICENSE_BIND"))
        return ItemLicense.Bind;

      if (equals("LICENSE_METALLIC"))
        return ItemLicense.Metallic;

      throw new Exception("Invalid license " + license);
    }

    private static Gender ParseGender(string gender)
    {
      Func<string, bool> equals = str => gender.Equals(str, StringComparison.InvariantCultureIgnoreCase);

      if (equals("all"))
        return Gender.None;

      if (equals("woman"))
        return Gender.Female;

      if (equals("man"))
        return Gender.Male;
      return Gender.None;
      //throw new Exception("Invalid gender "+ gender);
    }

    private static Gender ParseGender_2(string gender)
    {
      if (gender == "man")
        return Gender.Male;

      if (gender == "woman")
        return Gender.Female;

      if (gender == "unisex")
        return Gender.None;

      return Gender.None;
      //throw new Exception("Invalid gender "+ gender);
    }

    private static ItemInfo LoadAction(ItemNumber id, ItemInfoItemDto itemDto)
    {
      if (itemDto.action == null)
      {
        Logger.Warning("Missing action for item {id}", id);
        return new ItemInfoAction();
      }

      var item = new ItemInfoAction
      {
        RequiredMP = float.Parse(itemDto.action.ability.required_mp, CultureInfo.InvariantCulture),
        DecrementMP = float.Parse(itemDto.action.ability.decrement_mp, CultureInfo.InvariantCulture),
        DecrementMPDelay = float.Parse(itemDto.action.ability.decrement_mp_delay, CultureInfo.InvariantCulture)
      };

      if (itemDto.action.@float != null)
        item.ValuesF = itemDto.action.@float
            .Select(f => float.Parse(f.value.Replace("f", ""), CultureInfo.InvariantCulture)).ToList();

      if (itemDto.action.integer != null)
        item.Values = itemDto.action.integer.Select(i => i.value).ToList();

      return item;
    }

    private static ItemInfo LoadWeapon(ItemNumber id, ItemInfoItemDto itemDto)
    {
      if (itemDto.weapon == null)
        return new ItemInfoWeapon();

      var ability = itemDto.weapon.ability;
      var item = new ItemInfoWeapon
      {
        Type = ability.type,
        RateOfFire = float.Parse(ability.rate_of_fire, CultureInfo.InvariantCulture),
        Power = float.Parse(ability.power, CultureInfo.InvariantCulture),
        MoveSpeedRate = float.Parse(ability.move_speed_rate, CultureInfo.InvariantCulture),
        AttackMoveSpeedRate = float.Parse(ability.attack_move_speed_rate, CultureInfo.InvariantCulture),
        MagazineCapacity = ability.magazine_capacity,
        CrackedMagazineCapacity = ability.cracked_magazine_capacity,
        MaxAmmo = ability.max_ammo,
        Accuracy = float.Parse(ability.accuracy, CultureInfo.InvariantCulture),
        Range = string.IsNullOrWhiteSpace(ability.range)
              ? 0
              : float.Parse(ability.range, CultureInfo.InvariantCulture),
        SupportSniperMode = ability.support_sniper_mode > 0,
        SniperModeFov = ability.sniper_mode_fov > 0,
        AutoTargetDistance = ability.auto_target_distance == null
              ? 0
              : float.Parse(ability.auto_target_distance, CultureInfo.InvariantCulture)
      };

      if (itemDto.weapon.@float != null)
        item.ValuesF = itemDto.weapon.@float
            .Select(f => float.Parse(f.value.Replace("f", ""), CultureInfo.InvariantCulture)).ToList();

      if (itemDto.weapon.integer != null)
        item.Values = itemDto.weapon.integer.Select(i => i.value).ToList();

      return item;
    }

    public IEnumerable<ItemNumber> GetWorkingCapsules()
    {
      var dto = Deserialize<AddCapsuleDto>("xml/_eu_item_tooltip_addcapsule.x7");
      var capsules = new List<ItemNumber>();

      foreach (var capsule in dto.Item)
      {
        var hasItem = false;

        var item = capsule.Capsule_icon;
        var color = capsule.Color_index;
        var effect = capsule.Capsule_info;
        var slot = capsule.Capsule_slot;

        var items = new ConcurrentDictionary<ItemNumber, int>();
        var effects = new ConcurrentDictionary<int, List<CapsuleReward>>(); // Todo
        var slots = new ConcurrentStack<int>();
        var colors = new ConcurrentDictionary<int, int>();

        slots.Push(0);

        // Prepare Slots

        #region Prepare Slots

        if (int.TryParse(slot.Slot_1, out var Slot_1))
        {
          effects.TryAdd(Slot_1, new List<CapsuleReward>());
          slots.Push(Slot_1);
        }

        if (int.TryParse(slot.Slot_2, out var Slot_2))
        {
          effects.TryAdd(Slot_2, new List<CapsuleReward>());
          slots.Push(Slot_2);
        }

        if (int.TryParse(slot.Slot_3, out var Slot_3))
        {
          effects.TryAdd(Slot_3, new List<CapsuleReward>());
          slots.Push(Slot_3);
        }

        if (int.TryParse(slot.Slot_4, out var Slot_4))
        {
          effects.TryAdd(Slot_4, new List<CapsuleReward>());
          slots.Push(Slot_4);
        }

        if (int.TryParse(slot.Slot_5, out var Slot_5))
        {
          effects.TryAdd(Slot_5, new List<CapsuleReward>());
          slots.Push(Slot_5);
        }

        if (int.TryParse(slot.Slot_6, out var Slot_6))
        {
          effects.TryAdd(Slot_6, new List<CapsuleReward>());
          slots.Push(Slot_6);
        }

        if (int.TryParse(slot.Slot_7, out var Slot_7))
        {
          effects.TryAdd(Slot_7, new List<CapsuleReward>());
          slots.Push(Slot_7);
        }

        if (int.TryParse(slot.Slot_8, out var Slot_8))
        {
          effects.TryAdd(Slot_8, new List<CapsuleReward>());
          slots.Push(Slot_8);
        }

        if (int.TryParse(slot.Slot_9, out var Slot_9))
        {
          effects.TryAdd(Slot_9, new List<CapsuleReward>());
          slots.Push(Slot_9);
        }

        if (int.TryParse(slot.Slot_10, out var Slot_10))
        {
          effects.TryAdd(Slot_10, new List<CapsuleReward>());
          slots.Push(Slot_10);
        }

        if (int.TryParse(slot.Slot_11, out var Slot_11))
        {
          effects.TryAdd(Slot_11, new List<CapsuleReward>());
          slots.Push(Slot_11);
        }

        if (int.TryParse(slot.Slot_15, out var Slot_15))
        {
          effects.TryAdd(Slot_15, new List<CapsuleReward>());
          slots.Push(Slot_15);
        }

        if (int.TryParse(slot.Slot_16, out var Slot_16))
        {
          effects.TryAdd(Slot_16, new List<CapsuleReward>());
          slots.Push(Slot_16);
        }

        if (int.TryParse(slot.Slot_14, out var Slot_14))
        {
          effects.TryAdd(Slot_14, new List<CapsuleReward>());
          slots.Push(Slot_14);
        }

        if (int.TryParse(slot.Slot_12, out var Slot_12))
        {
          effects.TryAdd(Slot_12, new List<CapsuleReward>());
          slots.Push(Slot_12);
        }

        if (int.TryParse(slot.Slot_13, out var Slot_13))
        {
          effects.TryAdd(Slot_13, new List<CapsuleReward>());
          slots.Push(Slot_13);
        }

        #endregion

        #region Read Rewards 

        if (effects.TryGetValue(Slot_1, out var List_1) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_1),
                out CapsuleReward Effect_1))
          List_1.Add(Effect_1);

        if (effects.TryGetValue(Slot_2, out var List_2) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_2),
                out CapsuleReward Effect_2))
          List_2.Add(Effect_2);

        if (effects.TryGetValue(Slot_3, out var List_3) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_3),
                out CapsuleReward Effect_3))
          List_3.Add(Effect_3);

        if (effects.TryGetValue(Slot_4, out var List_4) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_4),
                out CapsuleReward Effect_4))
          List_4.Add(Effect_4);

        if (effects.TryGetValue(Slot_5, out var List_5) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_5),
                out CapsuleReward Effect_5))
          List_5.Add(Effect_5);

        if (effects.TryGetValue(Slot_6, out var List_6) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_6),
                out CapsuleReward Effect_6))
          List_6.Add(Effect_6);

        if (effects.TryGetValue(Slot_7, out var List_7) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_7),
                out CapsuleReward Effect_7))
          List_7.Add(Effect_7);

        if (effects.TryGetValue(Slot_8, out var List_8) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_8),
                out CapsuleReward Effect_8))
          List_8.Add(Effect_8);

        if (effects.TryGetValue(Slot_9, out var List_9) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_9),
                out CapsuleReward Effect_9))
          List_9.Add(Effect_9);

        if (effects.TryGetValue(Slot_10, out var List_10) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_10),
                out CapsuleReward Effect_10))
          List_10.Add(Effect_10);

        if (effects.TryGetValue(Slot_11, out var List_11) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_11),
                out CapsuleReward Effect_11))
          List_11.Add(Effect_11);

        if (effects.TryGetValue(Slot_14, out var List_14) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_14),
                out CapsuleReward Effect_14))
          List_14.Add(Effect_14);

        if (effects.TryGetValue(Slot_15, out var List_15) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_15),
                out CapsuleReward Effect_15))
          List_15.Add(Effect_15);

        if (effects.TryGetValue(Slot_16, out var List_16) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_16),
                out CapsuleReward Effect_16))
          List_16.Add(Effect_16);

        if (effects.TryGetValue(Slot_12, out var List_12) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_12),
                out CapsuleReward Effect_12))
          List_12.Add(Effect_12);

        if (effects.TryGetValue(Slot_13, out var List_13) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_13),
                out CapsuleReward Effect_13))
          List_13.Add(Effect_13);

        #endregion

        // Read Items

        #region Read Items

        if (int.TryParse(item.ID_1, out var ID_1))
          hasItem = true;

        if (int.TryParse(item.ID_2, out var ID_2))
          hasItem = true;

        if (int.TryParse(item.ID_3, out var ID_3))
          hasItem = true;

        if (int.TryParse(item.ID_4, out var ID_4))
          hasItem = true;

        if (int.TryParse(item.ID_5, out var ID_5))
          hasItem = true;

        if (int.TryParse(item.ID_6, out var ID_6))
          hasItem = true;

        if (int.TryParse(item.ID_7, out var ID_7))
          hasItem = true;

        if (int.TryParse(item.ID_8, out var ID_8))
          hasItem = true;

        if (int.TryParse(item.ID_9, out var ID_9))
          hasItem = true;

        if (int.TryParse(item.ID_10, out var ID_10))
          hasItem = true;

        if (int.TryParse(item.ID_11, out var ID_11))
          hasItem = true;

        if (int.TryParse(item.ID_15, out var ID_15))
          hasItem = true;

        if (int.TryParse(item.ID_16, out var ID_16))
          hasItem = true;

        if (int.TryParse(item.ID_14, out var ID_14))
          hasItem = true;

        if (int.TryParse(item.ID_12, out var ID_12))
          hasItem = true;

        if (int.TryParse(item.ID_13, out var ID_13))
          hasItem = true;

        #endregion

        if (hasItem)
          capsules.Add(int.Parse(capsule.Id));
      }

      return capsules;
    }

    public IEnumerable<AddCapsule> LoadCapsules()
    {
      var dto = Deserialize<AddCapsuleDto>("xml/_eu_item_tooltip_addcapsule.x7");

      var caps = new ConcurrentDictionary<ItemNumber, AddCapsule>();

      foreach (var capsule in dto.Item)
      {
        var retval = new AddCapsule(int.Parse(capsule.Id));

        var item = capsule.Capsule_icon;
        var color = capsule.Color_index;
        var effect = capsule.Capsule_info;
        var slot = capsule.Capsule_slot;

        var items = new ConcurrentDictionary<ItemNumber, int>();
        var effects = new ConcurrentDictionary<int, List<CapsuleReward>>(); // Todo
        var slots = new ConcurrentStack<int>();
        var colors = new ConcurrentDictionary<int, int>();
        var names = new ConcurrentDictionary<int, List<string>>();

        slots.Push(0);
        effects.TryAdd(0, new List<CapsuleReward>());

        // Prepare Slots

        #region Prepare Slots

        if (int.TryParse(slot.Slot_1, out var Slot_1))
        {
          effects.TryAdd(Slot_1, new List<CapsuleReward>());
          slots.Push(Slot_1);
        }

        if (int.TryParse(slot.Slot_2, out var Slot_2))
        {
          effects.TryAdd(Slot_2, new List<CapsuleReward>());
          slots.Push(Slot_2);
        }

        if (int.TryParse(slot.Slot_3, out var Slot_3))
        {
          effects.TryAdd(Slot_3, new List<CapsuleReward>());
          slots.Push(Slot_3);
        }

        if (int.TryParse(slot.Slot_4, out var Slot_4))
        {
          effects.TryAdd(Slot_4, new List<CapsuleReward>());
          slots.Push(Slot_4);
        }

        if (int.TryParse(slot.Slot_5, out var Slot_5))
        {
          effects.TryAdd(Slot_5, new List<CapsuleReward>());
          slots.Push(Slot_5);
        }

        if (int.TryParse(slot.Slot_6, out var Slot_6))
        {
          effects.TryAdd(Slot_6, new List<CapsuleReward>());
          slots.Push(Slot_6);
        }

        if (int.TryParse(slot.Slot_7, out var Slot_7))
        {
          effects.TryAdd(Slot_7, new List<CapsuleReward>());
          slots.Push(Slot_7);
        }

        if (int.TryParse(slot.Slot_8, out var Slot_8))
        {
          effects.TryAdd(Slot_8, new List<CapsuleReward>());
          slots.Push(Slot_8);
        }

        if (int.TryParse(slot.Slot_9, out var Slot_9))
        {
          effects.TryAdd(Slot_9, new List<CapsuleReward>());
          slots.Push(Slot_9);
        }

        if (int.TryParse(slot.Slot_10, out var Slot_10))
        {
          effects.TryAdd(Slot_10, new List<CapsuleReward>());
          slots.Push(Slot_10);
        }

        if (int.TryParse(slot.Slot_11, out var Slot_11))
        {
          effects.TryAdd(Slot_11, new List<CapsuleReward>());
          slots.Push(Slot_11);
        }

        if (int.TryParse(slot.Slot_15, out var Slot_15))
        {
          effects.TryAdd(Slot_15, new List<CapsuleReward>());
          slots.Push(Slot_15);
        }

        if (int.TryParse(slot.Slot_16, out var Slot_16))
        {
          effects.TryAdd(Slot_16, new List<CapsuleReward>());
          slots.Push(Slot_16);
        }

        if (int.TryParse(slot.Slot_14, out var Slot_14))
        {
          effects.TryAdd(Slot_14, new List<CapsuleReward>());
          slots.Push(Slot_14);
        }

        if (int.TryParse(slot.Slot_12, out var Slot_12))
        {
          effects.TryAdd(Slot_12, new List<CapsuleReward>());
          slots.Push(Slot_12);
        }

        if (int.TryParse(slot.Slot_13, out var Slot_13))
        {
          effects.TryAdd(Slot_13, new List<CapsuleReward>());
          slots.Push(Slot_13);
        }

        #endregion

        // Read Colors

        #region Read Colors 

        if (int.TryParse(color.Color_1, out var Color_1))
          colors.TryAdd(Slot_1, Color_1);

        if (int.TryParse(color.Color_2, out var Color_2))
          colors.TryAdd(Slot_2, Color_2);

        if (int.TryParse(color.Color_3, out var Color_3))
          colors.TryAdd(Slot_3, Color_3);

        if (int.TryParse(color.Color_4, out var Color_4))
          colors.TryAdd(Slot_4, Color_4);

        if (int.TryParse(color.Color_5, out var Color_5))
          colors.TryAdd(Slot_5, Color_5);

        if (int.TryParse(color.Color_6, out var Color_6))
          colors.TryAdd(Slot_6, Color_6);

        if (int.TryParse(color.Color_7, out var Color_7))
          colors.TryAdd(Slot_7, Color_7);

        if (int.TryParse(color.Color_8, out var Color_8))
          colors.TryAdd(Slot_8, Color_8);

        if (int.TryParse(color.Color_9, out var Color_9))
          colors.TryAdd(Slot_9, Color_9);

        if (int.TryParse(color.Color_10, out var Color_10))
          colors.TryAdd(Slot_10, Color_10);

        if (int.TryParse(color.Color_16, out var Color_16))
          colors.TryAdd(Slot_16, Color_16);

        #endregion

        // Read Rewards

        #region Read Rewards 

        if (effects.TryGetValue(Slot_1, out var List_1) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_1),
                out CapsuleReward Effect_1))
          List_1.Add(Effect_1);

        if (effects.TryGetValue(Slot_2, out var List_2) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_2),
                out CapsuleReward Effect_2))
          List_2.Add(Effect_2);

        if (effects.TryGetValue(Slot_3, out var List_3) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_3),
                out CapsuleReward Effect_3))
          List_3.Add(Effect_3);

        if (effects.TryGetValue(Slot_4, out var List_4) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_4),
                out CapsuleReward Effect_4))
          List_4.Add(Effect_4);

        if (effects.TryGetValue(Slot_5, out var List_5) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_5),
                out CapsuleReward Effect_5))
          List_5.Add(Effect_5);

        if (effects.TryGetValue(Slot_6, out var List_6) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_6),
                out CapsuleReward Effect_6))
          List_6.Add(Effect_6);

        if (effects.TryGetValue(Slot_7, out var List_7) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_7),
                out CapsuleReward Effect_7))
          List_7.Add(Effect_7);

        if (effects.TryGetValue(Slot_8, out var List_8) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_8),
                out CapsuleReward Effect_8))
          List_8.Add(Effect_8);

        if (effects.TryGetValue(Slot_9, out var List_9) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_9),
                out CapsuleReward Effect_9))
          List_9.Add(Effect_9);

        if (effects.TryGetValue(Slot_10, out var List_10) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_10),
                out CapsuleReward Effect_10))
          List_10.Add(Effect_10);

        if (effects.TryGetValue(Slot_11, out var List_11) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_11),
                out CapsuleReward Effect_11))
          List_11.Add(Effect_11);

        if (effects.TryGetValue(Slot_14, out var List_14) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_14),
                out CapsuleReward Effect_14))
          List_14.Add(Effect_14);

        if (effects.TryGetValue(Slot_15, out var List_15) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_15),
                out CapsuleReward Effect_15))
          List_15.Add(Effect_15);

        if (effects.TryGetValue(Slot_16, out var List_16) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_16),
                out CapsuleReward Effect_16))
          List_16.Add(Effect_16);

        if (effects.TryGetValue(Slot_12, out var List_12) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_12),
                out CapsuleReward Effect_12))
          List_12.Add(Effect_12);

        if (effects.TryGetValue(Slot_13, out var List_13) &&
            Enum.TryParse(AddCapsule.ConvertCapsuleReward(effect.Effect_key_13),
                out CapsuleReward Effect_13))
          List_13.Add(Effect_13);

        #endregion

        // Read Items

        #region Read Items

        if (int.TryParse(item.ID_1, out var ID_1))
          items.TryAdd(ID_1, Slot_1);

        if (int.TryParse(item.ID_2, out var ID_2))
          items.TryAdd(ID_2, Slot_2);

        if (int.TryParse(item.ID_3, out var ID_3))
          items.TryAdd(ID_3, Slot_3);

        if (int.TryParse(item.ID_4, out var ID_4))
          items.TryAdd(ID_4, Slot_4);

        if (int.TryParse(item.ID_5, out var ID_5))
          items.TryAdd(ID_5, Slot_5);

        if (int.TryParse(item.ID_6, out var ID_6))
          items.TryAdd(ID_6, Slot_6);

        if (int.TryParse(item.ID_7, out var ID_7))
          items.TryAdd(ID_7, Slot_7);

        if (int.TryParse(item.ID_8, out var ID_8))
          items.TryAdd(ID_8, Slot_8);

        if (int.TryParse(item.ID_9, out var ID_9))
          items.TryAdd(ID_9, Slot_9);

        if (int.TryParse(item.ID_10, out var ID_10))
          items.TryAdd(ID_10, Slot_10);

        if (int.TryParse(item.ID_11, out var ID_11))
          items.TryAdd(ID_11, Slot_11);

        if (int.TryParse(item.ID_15, out var ID_15))
          items.TryAdd(ID_15, Slot_15);

        if (int.TryParse(item.ID_16, out var ID_16))
          items.TryAdd(ID_16, Slot_16);

        if (int.TryParse(item.ID_14, out var ID_14))
          items.TryAdd(ID_14, Slot_14);

        if (int.TryParse(item.ID_12, out var ID_12))
          items.TryAdd(ID_12, Slot_12);

        if (int.TryParse(item.ID_13, out var ID_13))
          items.TryAdd(ID_13, Slot_13);

        #endregion

        var prizes = new List<AddCapsuleReward>();

        foreach (var iSlot in slots)
          prizes.Add(new AddCapsuleReward(iSlot));

        var shop = GameServer.Instance.ResourceCache.GetShop();
        var xitems = GameServer.Instance.ResourceCache.GetItems();

        foreach (var iItem in items)
        {
          if (slots.Contains(iItem.Value))
          {
            var xitem = xitems.FirstOrDefault(x => x.Key == iItem.Key);

            if (xitem.Value == null)
              continue;

            if (xitem.Value?.Name.Contains("(7/15/30") ?? false)
            {
              var xitem2 = xitems.FirstOrDefault(x =>
                  x.Value.Name.Trim().Contains(
                      xitem.Value?.Name
                          .Replace("(7/15/30/Permanent)", "(Permanent)")
                          .Replace("(7/15/30)", "(Permanent)")
                          .Replace("(7/15/30 Days/perm)", "(perm)")
                          .Trim()));

              if (xitem2.Value == null)
              {
                xitem2 = xitems.FirstOrDefault(x =>
                    x.Value.Name.Trim().Contains(
                        xitem.Value?.Name
                            .Replace("(7/15/30/Permanent)", " (Permanent)")
                            .Replace("(7/15/30)", " (Permanent)")
                            .Replace("(7/15/30 Days/perm)", " (perm)")
                            .Trim()));
              }

              if (xitem2.Value == null)
              {
                xitem2 = xitems.FirstOrDefault(x =>
                    x.Value.Name.Trim().Equals(
                        xitem.Value?.Name
                            .Replace("(7/15/30/Permanent)", string.Empty)
                            .Replace("(7/15/30)", string.Empty)
                            .Replace("(7/15/30 Days/perm)", string.Empty)
                            .Trim()));
              }

              if (xitem2.Value == null)
              {
                xitem2 = xitems.FirstOrDefault(x =>
                    x.Value.Name.Trim().Contains(
                        xitem.Value?.Name
                            .Replace("(AP)", " ")
                            .Replace("(7/15/30/Permanent)", "(Permanent)")
                            .Replace("(7/15/30)", "(Permanent)")
                            .Replace("(7/15/30 Days/perm)", "(perm)")
                            .Trim()));
              }

              if (xitem2.Value == null)
              {
                xitem2 = xitems.FirstOrDefault(x =>
                    x.Value.Name.Trim().Contains(
                        xitem.Value?.Name
                            .Replace("(AP)", " ")
                            .Replace("(7/15/30/Permanent)", " (Permanent)")
                            .Replace("(7/15/30)", " (Permanent)")
                            .Replace("(7/15/30 Days/perm)", " (perm)")
                            .Trim()));
              }

              if (xitem2.Value == null)
              {
                xitem2 = xitems.FirstOrDefault(x =>
                    x.Value.Name.Trim().Equals(
                        xitem.Value?.Name
                            .Replace("(AP)", " ")
                            .Replace("(7/15/30/Permanent)", string.Empty)
                            .Replace("(7/15/30)", string.Empty)
                            .Replace("(7/15/30 Days/perm)", string.Empty)
                            .Trim()));
              }

              if (xitem2.Value != null)
                items.TryAdd(xitem2.Key, iItem.Value);

              continue;
            }

            if (shop.Items.TryGetValue(iItem.Key, out var shopItem))
            {
              var prize = prizes.FirstOrDefault(x => x.SlotId == iItem.Value);
              if (prize != null)
              {
                colors.TryGetValue(iItem.Value, out var iColor);

                var colorAvailable =
                    shop.Items.Any(x => x.Key == iItem.Key && x.Value.ColorGroup > iColor);
                if (!colorAvailable)
                  iColor = 0;

                var existing = false;
                if (!names.ContainsKey(prize.SlotId))
                  names.TryAdd(prize.SlotId, new List<string>());
                names.TryGetValue(prize.SlotId, out var namelist);

                var name = xitem.Value?.Name
                    .Replace("(1 Day)", $"{prize.SlotId}-T")
                    .Replace("(7 Days)", $"{prize.SlotId}-T")
                    .Replace("(15 Days)", $"{prize.SlotId}-T")
                    .Replace("(30 Days)", $"{prize.SlotId}-T")
                    .Replace("(Permanent)", $"{prize.SlotId}-T")
                    .Trim() ?? "";

                if (namelist.Contains(name))
                  existing = true;
                else
                  namelist.Add(name);

                if (!existing)
                {
                  prize.Items.Add(shopItem, iColor);
                  retval.Ready = true;
                }
              }
            }
          }
        }

        foreach (var iEffect in effects)
        {
          if (slots.Contains(iEffect.Key))
          {
            var prize = prizes.FirstOrDefault(x => x.SlotId == iEffect.Key);
            prize.Rewards.AddRange(iEffect.Value);
          }
        }

        foreach (var iPrize in prizes)
        {
          if (iPrize.Items.Any() || iPrize.Rewards.Any())
            retval.Prizes.TryAdd(iPrize.SlotId, iPrize);
        }

        if (retval.Prizes.Any())
          caps.TryAdd(retval.CapsuleItemId, retval);
      }

      return caps.Values;
    }

    public IEnumerable<CapsuleRewards> LoadItemRewards()
    {
      var dto = Deserialize<ItemRewardDto>("xml/ItemBag.xml");
      if (dto?.Items != null)
        foreach (var it in dto.Items)
        {
          var ret = new CapsuleRewards { Item = it.Number, Bags = new List<BagReward>() };

          if (it.Groups != null)
          {
            foreach (var group in it.Groups)
            {
              var bag = new BagReward
              {
                Bag = new List<ItemReward>()
              };

              if (group.Rewards != null)
              {
                foreach (var rw in group.Rewards)
                {
                  var PEN = (CapsuleRewardType)rw.Type == CapsuleRewardType.PEN ? rw.Value : 0;
                  var Period = (CapsuleRewardType)rw.Type == CapsuleRewardType.PEN ? 0 : rw.Value;

                  bag.Bag.Add(new ItemReward
                  {
                    Type = (CapsuleRewardType)rw.Type,
                    Item = rw.Data,
                    PriceType = (ItemPriceType)rw.PriceType,
                    PeriodType = (ItemPeriodType)rw.PeriodType,
                    Period = Period,
                    PEN = PEN,
                    Effects = rw.Effects.Split(",").Select(e => uint.Parse(e)).ToArray(),
                    Rate = rw.Rate,
                    Color = rw.Color,
                    Value = rw.Value
                  });
                }
              }

              ret.Bags.Add(bag);
            }
          }

          yield return ret;
        }
    }

    #endregion

    #region RandomShop

    public IEnumerable<RandomShopPool> LoadRandomShop()
    {
      var dto = Deserialize<RandomShopResourceDto>("xml/RandomShop.xml");
      if (dto?.Pools == null)
        yield break;

      foreach (var poolDto in dto.Pools)
      {
        var pool = new RandomShopPool
        {
          Id = poolDto.Id,
          PriceType = (ItemPriceType)poolDto.PriceType,
          Price = poolDto.Price
        };

        if (poolDto.Items != null)
        {
          foreach (var itemDto in poolDto.Items)
          {
            pool.Items.Add(new RandomShopItem
            {
              ItemNumber = itemDto.ItemNumber,
              PeriodType = (ItemPeriodType)itemDto.PeriodType,
              Period = (ushort)itemDto.Period,
              Effect = itemDto.Effect,
              Color = itemDto.Color,
              Rate = itemDto.Rate
            });
          }
        }

        yield return pool;
      }
    }

    #endregion

    #region CardSystem

    public CardSystemInfo LoadCardSystem()
    {
      var dto = Deserialize<CardSystemResourceDto>("xml/_eu_card_system_info.x7");
      if (dto == null)
        return new CardSystemInfo { Active = false };

      var info = new CardSystemInfo
      {
        Active = dto.active,
        CurrentSeason = dto.CurrentSeason?.num ?? 2
      };

      if (dto.Formula != null)
      {
        info.Formula = new CardFormula
        {
          PlayLimitTime = dto.Formula.play_limit_time,
          PlayLimitMinCount = dto.Formula.play_limit_min_count,
          PlayDefaultTime = dto.Formula.play_default_time,
          PlayDefaultCount = dto.Formula.play_default_count,
          GamblePen = dto.Formula.gamble_pen,
          GambleLimitMinCount = dto.Formula.gamble_limit_min_count,
          CompleteCardCount = dto.Formula.complete_card_count
        };
      }

      if (dto.Seasons != null)
      {
        foreach (var seasonDto in dto.Seasons)
        {
          var season = new CardSeasonInfo
          {
            Num = seasonDto.num,
            BuyCapsule = seasonDto.buy_capsule,
            ShopId = seasonDto.shop_id
          };

          if (seasonDto.Cards != null)
          {
            foreach (var cardDto in seasonDto.Cards)
            {
              season.Cards.Add(new CardEntry
              {
                Num = cardDto.num,
                ItemId = (ItemNumber)cardDto.item_id,
                ShopId = cardDto.shop_id,
                PeriodType = ParsePeriodType(cardDto.period_type),
                PeriodValue = cardDto.period_value,
                Color = cardDto.color,
                EffectId = cardDto.effect_id,
                Grade = cardDto.grade,
                PlayProb = cardDto.play_prob,
                TryProb = cardDto.try_prob
              });
            }
          }

          if (seasonDto.Reward != null)
          {
            season.Reward = new CardReward
            {
              ItemId = (ItemNumber)seasonDto.Reward.item_id,
              ShopId = seasonDto.Reward.shop_id,
              PeriodType = ParsePeriodType(seasonDto.Reward.period_type),
              PeriodValue = seasonDto.Reward.period_value,
              Color = seasonDto.Reward.color,
              EffectId = seasonDto.Reward.effect_id
            };
          }

          info.Seasons.Add(season);
        }
      }

      return info;
    }

    private static ItemPeriodType ParsePeriodType(string periodType)
    {
      if (string.IsNullOrEmpty(periodType))
        return ItemPeriodType.None;
      if (periodType == "USECNT")
        return ItemPeriodType.Units;
      if (periodType == "USEDAY")
        return ItemPeriodType.Days;
      if (periodType == "USEHR")
        return ItemPeriodType.Hours;
      return ItemPeriodType.None;
    }

    #endregion

    #region ExperienceBonus

    public ExperienceBonusConfig LoadExperienceBonus()
    {
      var dto = Deserialize<ExperienceBonusDto>("xml/experience_bonus.x7");
      if (dto == null)
        return new ExperienceBonusConfig();

      var config = new ExperienceBonusConfig();

      if (dto.Football != null)
        config.Touchdown = MapExpBonus(dto.Football);
      if (dto.DeathMatch != null)
        config.Deathmatch = MapExpBonus(dto.DeathMatch);
      if (dto.Survival != null)
        config.Survival = MapExpBonus(dto.Survival);
      if (dto.Captain != null)
        config.Captain = MapExpBonus(dto.Captain);
      if (dto.Seize != null)
        config.Seize = MapExpBonus(dto.Seize);
      if (dto.Horde != null)
        config.Horde = MapExpBonus(dto.Horde);
      if (dto.Slaughter != null)
        config.Slaughter = MapExpBonus(dto.Slaughter);
      if (dto.FreeForAll != null)
        config.FreeForAll = MapExpBonus(dto.FreeForAll);
      // Map survival to chaser/battleroyal as fallback
      config.Chaser = config.Survival;
      config.BattleRoyal = config.Survival;
      config.SnowballFight = config.Deathmatch;

      return config;
    }

    private static ExperienceBonusEntry MapExpBonus(ExperienceBonusModeDto dto)
    {
      return new ExperienceBonusEntry
      {
        RankingFactor = dto.ranking_factor,
        PlayerCountFactor = dto.player_count_factor,
        VariableExpPerMin = dto.variable_exp_per_min,
        ConstantExpPerMin = dto.constant_exp_per_min,
        DamageRanking1stPoint = dto.damageranking_1st_point,
        DamageRanking2ndPoint = dto.damageranking_2nd_point,
        DamageRanking3rdPoint = dto.damageranking_3rd_point,
        IsValid = dto.variable_exp_per_min > 0 || dto.constant_exp_per_min > 0
      };
    }

    #endregion

    #region PointBonus

    public PointBonusConfig LoadPointBonus()
    {
      var dto = Deserialize<PointBonusDto>("xml/point_bonus.x7");
      if (dto == null)
        return new PointBonusConfig();

      var config = new PointBonusConfig();

      if (dto.Football != null)
        config.Touchdown = MapPointBonus(dto.Football);
      if (dto.DeathMatch != null)
        config.Deathmatch = MapPointBonus(dto.DeathMatch);
      if (dto.Survival != null)
        config.Survival = MapPointBonus(dto.Survival);
      if (dto.Captain != null)
        config.Captain = MapPointBonus(dto.Captain);
      if (dto.Arcade != null)
      {
        config.Arcade = new PointBonusEntry
        {
          PointPerMin = dto.Arcade.point_per_min,
          IsValid = dto.Arcade.point_per_min > 0
        };
      }
      config.Chaser = config.Survival;
      config.BattleRoyal = config.Survival;
      config.SnowballFight = config.Deathmatch;
      config.Horde = config.Survival;

      if (dto.LevelBonuses != null)
      {
        foreach (var levelDto in dto.LevelBonuses)
        {
          config.LevelBonuses.Add(new PointBonusLevelEntry
          {
            Min = levelDto.min,
            Max = levelDto.max,
            PenBonus = levelDto.pen_bonus
          });
        }
      }

      return config;
    }

    private static PointBonusEntry MapPointBonus(PointBonusModeDto dto)
    {
      return new PointBonusEntry
      {
        RankingFactor = dto.ranking_factor,
        PlayerCountFactor = dto.player_count_factor,
        PointPerMin = dto.point_per_min,
        IsValid = dto.point_per_min > 0
      };
    }

    #endregion

    #region MasterExperience

    public MasterExperience LoadMasterExperience()
    {
      var dto = Deserialize<MasterExperienceDto>("xml/master_experience.x7");
      if (dto?.Entries == null)
        return new MasterExperience { MaxLevel = 50, Entries = System.Array.Empty<MasterExperienceEntry>() };

      var entries = new MasterExperienceEntry[dto.Entries.Length];
      for (var i = 0; i < dto.Entries.Length; i++)
      {
        entries[i] = new MasterExperienceEntry
        {
          Level = i,
          Require = (uint)dto.Entries[i].require,
          Accumulate = (uint)dto.Entries[i].accumulate
        };
      }

      return new MasterExperience
      {
        MaxLevel = dto.maxLevel,
        Entries = entries
      };
    }

    #endregion

    #region BurningTime

    public BurningTimeInfo LoadBurningTime()
    {
      var dto = Deserialize<BurningTimeDto>("xml/burning_time.x7");
      return MapBurningTime(dto);
    }

    public BurningTimeInfo LoadBurningTimePve()
    {
      var dto = Deserialize<BurningTimeDto>("xml/burning_time_pve.x7");
      return MapBurningTime(dto);
    }

    private static BurningTimeInfo MapBurningTime(BurningTimeDto dto)
    {
      var dict = new Dictionary<uint, IReadOnlyList<BurningTimeEntry>>();
      if (dto?.Modes == null)
        return new BurningTimeInfo { Entries = dict };

      foreach (var mode in dto.Modes)
      {
        if (!dict.ContainsKey(mode.mode))
          dict[mode.mode] = new List<BurningTimeEntry>();

        var list = (List<BurningTimeEntry>)dict[mode.mode];
        list.Add(new BurningTimeEntry
        {
          Mode = mode.mode,
          LevelMin = mode.Condition?.ca_lv_min ?? 0,
          LevelMax = mode.Condition?.ca_lv_max ?? 0,
          Point = mode.Condition?.point ?? 0,
          BurningTime = mode.Condition?.burning_time ?? 0,
          MultiAp = mode.Status?.multi_ap ?? 1.0f,
          PlusAs = mode.Status?.plus_as ?? 0,
          AvHp = mode.Status?.av_hp ?? 100,
          MultiDp = mode.Status?.multi_dp ?? 0,
          AvSp = mode.Status?.av_sp ?? 100
        });
      }

      return new BurningTimeInfo { Entries = dict };
    }

    #endregion

    #region EquipLimit

    public EquipLimitInfo LoadEquipLimit()
    {
      var dto = Deserialize<EquipLimitDto>("xml/equip_limit.x7");
      var entries = new Dictionary<int, EquipLimitEntry>();
      if (dto?.Preset?.Entries == null)
        return new EquipLimitInfo { Entries = entries };

      foreach (var entryDto in dto.Preset.Entries)
      {
        var allowedItems = new HashSet<uint>();
        if (entryDto.Items != null)
          foreach (var item in entryDto.Items)
            allowedItems.Add(item.Item_Id);

        entries[entryDto.id] = new EquipLimitEntry
        {
          Id = entryDto.id,
          StringKey = entryDto.string_key,
          AllowedItems = allowedItems
        };
      }

      return new EquipLimitInfo { Entries = entries };
    }

    #endregion

    #region RoomOption

    public RoomOptionInfo LoadRoomOption()
    {
      var root = Deserialize<RoomOptionRootDto>("xml/room_option.x7");
      var dto = root?.Mode;
      if (dto == null)
        return new RoomOptionInfo { RewardConditionTime = 60, Modes = new List<RoomOptionModeEntry>(), ModeRewards = new List<RoomOptionRewardEntry>() };

      var modes = new List<RoomOptionModeEntry>();
      if (dto.ModeType?.Modes != null)
      {
        foreach (var modeDto in dto.ModeType.Modes)
        {
          modes.Add(new RoomOptionModeEntry
          {
            ModeId = modeDto.mode_id,
            Probability = modeDto.prob,
            ScoreLimit = modeDto.score,
            TimeLimit = modeDto.time,
            LimitPlayer = modeDto.limit_player,
            SpectatorCount = modeDto.spectator_count,
            LimitPlayTime = modeDto.limit_play_time
          });
        }
      }

      var rewards = new List<RoomOptionRewardEntry>();
      if (dto.ModeRewards != null)
      {
        foreach (var rewardDto in dto.ModeRewards)
        {
          var requitals = new List<RoomOptionRequitalEntry>();
          if (rewardDto.Requitals != null)
          {
            foreach (var reqDto in rewardDto.Requitals)
            {
              requitals.Add(new RoomOptionRequitalEntry
              {
                Key = reqDto.key,
                GiftType = reqDto.gift_type,
                ItemKey = reqDto.item_key,
                ShopId = reqDto.shop_id,
                PeriodType = reqDto.period_type,
                Period = reqDto.period,
                Color = reqDto.color,
                EffectId = reqDto.effect_id,
                Probability = reqDto.prob
              });
            }
          }

          rewards.Add(new RoomOptionRewardEntry
          {
            MinPlayer = rewardDto.min_player,
            Requitals = requitals
          });
        }
      }

      return new RoomOptionInfo
      {
        RewardConditionTime = dto.reward_condition_time,
        Modes = modes,
        ModeRewards = rewards
      };
    }

    #endregion

    #region EnchantData

    public EnchantInfo LoadEnchantData()
    {
      var dto = Deserialize<EnchantDataDto>("xml/enchant_data.x7");
      if (dto == null)
        return new EnchantInfo();

      var masteryNeeds = new Dictionary<string, IReadOnlyList<EnchantMasteryNeed>>();
      if (dto.MasteryTable?.Entries != null)
      {
        var groups = new Dictionary<string, List<EnchantMasteryNeed>>();
        foreach (var entry in dto.MasteryTable.Entries)
        {
          if (!groups.ContainsKey(entry.item_type))
            groups[entry.item_type] = new List<EnchantMasteryNeed>();
          groups[entry.item_type].Add(new EnchantMasteryNeed
          {
            ItemType = entry.item_type,
            EnchantCount = entry.enchant_cnt,
            Durability = entry.durability,
            Period = entry.period
          });
        }
        foreach (var kvp in groups)
          masteryNeeds[kvp.Key] = kvp.Value;
      }

      var prices = new Dictionary<string, IReadOnlyList<EnchantPrice>>();
      if (dto.PriceTable?.Entries != null)
      {
        var groups = new Dictionary<string, List<EnchantPrice>>();
        foreach (var entry in dto.PriceTable.Entries)
        {
          if (!groups.ContainsKey(entry.item_type))
            groups[entry.item_type] = new List<EnchantPrice>();
          groups[entry.item_type].Add(new EnchantPrice
          {
            ItemType = entry.item_type,
            EnchantCount = entry.enchant_cnt,
            Price = entry.enchant_price
          });
        }
        foreach (var kvp in groups)
          prices[kvp.Key] = kvp.Value;
      }

      return new EnchantInfo
      {
        MasteryPerMin = dto.Config?.Data?.mastery_per_min ?? 20,
        BonusProb = dto.Config?.Data?.bonus_prob ?? 500000,
        ProbUnit = dto.Config?.Data?.prob_unit ?? 10000000,
        NoticeEnchantCount = dto.Config?.Data?.notice_enchant_cnt ?? 15,
        MasteryNeeds = masteryNeeds,
        Prices = prices
      };
    }

    #endregion

    #region EnchantList

    public IReadOnlyList<EnchantEffect> LoadEnchantList()
    {
      var dto = Deserialize<EnchantListDto>("xml/enchant_list.x7");
      if (dto?.Entries == null)
        return new List<EnchantEffect>();

      var result = new List<EnchantEffect>();
      foreach (var entry in dto.Entries)
      {
        result.Add(new EnchantEffect
        {
          MainType = entry.main_type,
          Upper = entry.upper,
          Lower = entry.lower,
          EffectType = entry.EFFECT_TYPE,
          EffectCondition = entry.EFFECT_CONDITION,
          EffectLevel = entry.effect_level,
          SelectProbability = entry.select_prob,
          EffectKey = entry.effect_key,
          ValueMin = entry.Value_Min,
          ValueMax = entry.Value_max,
          RateMin = entry.Rate_min,
          RateMax = entry.Rate_Max,
          ValueTime = entry.Value_time,
          Position = entry.POSITION
        });
      }

      return result;
    }

    #endregion

    #region EnchantExtractKey

    public IReadOnlyDictionary<uint, int> LoadEnchantExtractKey()
    {
      var dto = Deserialize<EnchantExtractKeyDto>("xml/enchant_extractkey.x7");
      if (dto?.Entries == null)
        return new Dictionary<uint, int>();

      var result = new Dictionary<uint, int>();
      foreach (var entry in dto.Entries)
        result[entry.key] = entry.extracting_key;

      return result;
    }

    #endregion

    #region EsperEnchantPrice

    public IReadOnlyList<EsperEnchantPriceEntry> LoadEsperEnchantPrice()
    {
      var dto = Deserialize<EsperEnchantPriceDto>("xml/esper_enchant_price.x7");
      if (dto?.Entries == null)
        return new List<EsperEnchantPriceEntry>();

      var result = new List<EsperEnchantPriceEntry>();
      foreach (var entry in dto.Entries)
      {
        result.Add(new EsperEnchantPriceEntry
        {
          Index = entry.INDEX,
          Price = entry.PRICE,
          SuccessProbability = entry.SUCCESS_PROB,
          FailKeep = entry.FAIL_KEEP,
          FailDown = entry.FAIL_DOWN,
          FailDestruction = entry.FAIL_DESTRUCTION
        });
      }

      return result;
    }

    #endregion

    #region ItemGrade

    public ItemGradeInfo LoadItemGrade()
    {
      var dto = Deserialize<ItemGradeDto>("xml/item_grade.x7");
      var dict = new Dictionary<uint, string>();
      if (dto?.Mode?.Conditions != null)
      {
        foreach (var condition in dto.Mode.Conditions)
          dict[condition.effect_id] = condition.item_grade;
      }

      return new ItemGradeInfo { EffectToGrade = dict };
    }

    #endregion

    #region CombineElement

    public CombineElementInfo LoadCombineElement()
    {
      var dto = Deserialize<CombineElementDto>("xml/combine_element_info.x7");
      if (dto?.Values == null)
        return new CombineElementInfo { Entries = new List<CombineElementEntry>() };

      var entries = new List<CombineElementEntry>();
      foreach (var value in dto.Values)
      {
        entries.Add(new CombineElementEntry
        {
          ItemKey = value.item_key,
          UiSlot = value.ui_slot,
          Use = value.use == "on"
        });
      }

      return new CombineElementInfo { Entries = entries };
    }

    #endregion

    #region DecompositionElement

    public DecompositionElementInfo LoadDecompositionElement()
    {
      var dto = Deserialize<DecompositionElementDto>("xml/decomposition_element_info.x7");
      if (dto?.Values == null)
        return new DecompositionElementInfo { Entries = new List<DecompositionElementEntry>() };

      var entries = new List<DecompositionElementEntry>();
      foreach (var value in dto.Values)
      {
        entries.Add(new DecompositionElementEntry
        {
          ItemKey = value.item_key,
          UiSlot = value.ui_slot
        });
      }

      return new DecompositionElementInfo { Entries = entries };
    }

    #endregion

    #region SeizeModeNewInfo

    public SeizeModeInfo LoadSeizeModeNewInfo()
    {
      var dto = Deserialize<SeizeModeNewInfoDto>("xml/seize_mode_newinfo.x7");
      if (dto?.Foothold == null)
        return new SeizeModeInfo();

      var f = dto.Foothold;
      return new SeizeModeInfo
      {
        Gauge = f.Base?.gauge ?? 30000,
        GaugeUpDelay = f.Base?.gauge_up_delay ?? 1000,
        CoreFootholder = f.Base?.core_footholder ?? 3000,
        AssistFootholder = f.Base?.assist_footholder ?? 1000,
        ResetOnLeaveOrDeath = f.Base?.reset ?? true,
        PointPerCapture = f.Score?.point ?? 5,
        AssistPointPerCapture = f.Score?.assist_point ?? 1,
        UpkeepEnabled = f.Upkeep?.actived ?? true,
        UpkeepDelay = f.Upkeep?.delay ?? 60000,
        UpkeepScore = f.Upkeep?.score ?? 10,
        TimeBonusEnabled = f.TimeBonus?.actived ?? true,
        TimeBonusDelay = f.TimeBonus?.delay ?? 60000,
        TimeBonusDefault = f.TimeBonus?.default_bonus ?? 0,
        TimeBonusAdd = f.TimeBonus?.add_bonus ?? 5,
        TimeBonusAddLimit = f.TimeBonus?.add_bonus_limit ?? 15
      };
    }

    #endregion

    #region StadiumInfo

    public StadiumInfo LoadStadiumInfo()
    {
      var dto = Deserialize<StadiumInfoDto>("xml/stadium_info.x7");
      if (dto?.MapInfos == null)
        return new StadiumInfo { MapInfos = new List<StadiumMapInfo>() };

      var mapInfos = new List<StadiumMapInfo>();
      foreach (var mapDto in dto.MapInfos)
      {
        var blastInfos = new List<StadiumBlastInfo>();
        if (mapDto.BlastInfos != null)
        {
          foreach (var blastDto in mapDto.BlastInfos)
          {
            blastInfos.Add(new StadiumBlastInfo
            {
              Index = blastDto.INDEX,
              Name = blastDto.NAME,
              DefaultHp = blastDto.DEFAULT_HP,
              Min = blastDto.MIN,
              Max = blastDto.MAX,
              IncHp = blastDto.INC_HP,
              UsePoint = blastDto.USE_POINT
            });
          }
        }

        mapInfos.Add(new StadiumMapInfo
        {
          MapId = mapDto.MAPID,
          Mode = mapDto.MODE,
          BlastInfos = blastInfos
        });
      }

      return new StadiumInfo { MapInfos = mapInfos };
    }

    #endregion

    #region DecompositionInfo

    public DecompositionInfo LoadDecompositionInfo()
    {
      var dto = Deserialize<DecompositionInfoDto>("xml/_eu_decomposition_info.x7");
      if (dto == null)
        return new DecompositionInfo();

      var methods = new List<DecompositionMethod>();
      if (dto.method != null)
      {
        foreach (var methodDto in dto.method)
        {
          var components = new List<DecompositionComponent>();
          if (methodDto.component != null)
          {
            foreach (var compDto in methodDto.component)
            {
              components.Add(new DecompositionComponent
              {
                Condition = compDto.condition,
                ItemKey = compDto.item_key,
                ShopId = compDto.shop_id,
                PeriodType = compDto.period_type,
                Period = compDto.period,
                Color = compDto.color,
                EffectId = compDto.effect_id
              });
            }
          }

          methods.Add(new DecompositionMethod
          {
            PeriodType = methodDto.period_type,
            EffectMinCount = methodDto.effect_min_cnt,
            EffectMaxCount = methodDto.effect_max_cnt,
            Use = methodDto.use == "on",
            Bonus = methodDto.bonus == "on",
            Components = components
          });
        }
      }

      var bonuses = new List<DecompositionBonus>();
      if (dto.bonus_data?.bonus != null)
      {
        foreach (var bonusDto in dto.bonus_data.bonus)
        {
          bonuses.Add(new DecompositionBonus
          {
            PeriodMultipleValue = bonusDto.period_multiple_value,
            ItemMainType = bonusDto.item_main_type,
            ItemSubType = bonusDto.item_sub_type
          });
        }
      }

      var prohibited = new List<uint>();
      if (dto.prohibition?.data != null)
      {
        foreach (var dataDto in dto.prohibition.data)
          prohibited.Add(dataDto.item_key);
      }

      return new DecompositionInfo
      {
        PenPrice = dto.pen_price,
        MinHours = dto.min_hours,
        MinDays = dto.min_days,
        Methods = methods,
        Bonuses = bonuses,
        ProhibitedItems = prohibited
      };
    }

    #endregion

    #region CombinationInfo

    public CombinationInfo LoadCombinationInfo()
    {
      var dto = Deserialize<CombinationInfoDto>("xml/_eu_combination_info.x7");
      if (dto == null)
        return new CombinationInfo();

      CombinationItem MapItemArgon(CombinationInfoArgonComponentDto d)
      {
        if (d == null) return null;
        return new CombinationItem
        {
          ItemKey = d.item_key,
          ShopId = d.shop_id,
          PeriodType = d.period_type,
          Period = d.period,
          Color = d.color,
          EffectId = d.effect_id
        };
      }

      CombinationItem MapItemKrypton(CombinationInfoKryptonComponentDto d)
      {
        if (d == null) return null;
        return new CombinationItem
        {
          ItemKey = d.item_key,
          ShopId = d.shop_id,
          PeriodType = d.period_type,
          Period = d.period,
          Color = d.color,
          EffectId = d.effect_id
        };
      }

      var components = new List<CombinationComponent>();
      if (dto.component != null)
      {
        foreach (var compDto in dto.component)
        {
          components.Add(new CombinationComponent
          {
            ItemKey = compDto.item_key,
            ShopId = compDto.shop_id,
            PeriodType = compDto.period_type,
            Period = compDto.period,
            Color = compDto.color,
            EffectId = compDto.effect_id,
            MinUseCount = compDto.min_use_cnt,
            MaxUseCount = compDto.max_use_cnt
          });
        }
      }

      CombinationEnchantOption enchantOption = null;
      if (dto.enchant_option != null)
      {
        var eo = dto.enchant_option;
        enchantOption = new CombinationEnchantOption
        {
          EnchantItemKey = eo.enchant_item_key,
          EnchantShopId = eo.enchant_shop_id,
          EnchantPeriodType = eo.enchant_period_type,
          EnchantPeriod = eo.enchant_period,
          ProtectItemKey = eo.protect_item_key,
          ProtectShopId = eo.protect_shop_id,
          ProtectPeriodType = eo.protect_period_type,
          ProtectPeriod = eo.protect_period
        };
      }

      return new CombinationInfo
      {
        PenPrice = dto.pen_price,
        MinHours = dto.min_hours,
        MinDays = dto.min_days,
        ArgonComponent = MapItemArgon(dto.argon_component),
        KryptonComponent = MapItemKrypton(dto.krypton_component),
        EnchantOption = enchantOption,
        OvercountMaxLevel = dto.overcount_weight?.max_level ?? 0,
        OvercountWeightMax = dto.overcount_weight?.weight_max ?? 0,
        Components = components
      };
    }

    #endregion

    #region MissionInfo

    public MissionInfo LoadMissionInfo()
    {
      var dto = Deserialize<MissionInfoDto>("xml/mission.x7");
      if (dto == null)
        return new MissionInfo();

      var config = dto.mission_config;
      var pvpMissions = new List<MissionEntry>();
      var pveMissions = new List<MissionEntry>();

      if (dto.daily_pvp_mission != null)
      {
        foreach (var mDto in dto.daily_pvp_mission)
          pvpMissions.Add(MapMissionEntry(mDto));
      }

      if (dto.daily_pve_mission != null)
      {
        foreach (var mDto in dto.daily_pve_mission)
          pveMissions.Add(MapMissionEntry(mDto));
      }

      return new MissionInfo
      {
        MissionCheckInterval = config?.mission_check_interval ?? 60,
        MaxMissionCount = config?.max_mission_count ?? 3,
        MissionRewardMailExpireDays = config?.mission_reward_mail_expire_days ?? 7,
        DailyPvpMissionCount = config?.daily_pvp_mission_count ?? 3,
        DailyPveMissionCount = config?.daily_pve_mission_count ?? 3,
        DailyMissionRewardMailExpireDays = config?.daily_mission_reward_mail_expire_days ?? 7,
        DailyPvpMissions = pvpMissions,
        DailyPveMissions = pveMissions
      };
    }

    private static MissionEntry MapMissionEntry(MissionInfoDailyPvpMissionDto dto)
    {
      var conditions = new List<MissionCondition>();
      if (dto.mission_condition != null)
      {
        foreach (var cDto in dto.mission_condition)
        {
          conditions.Add(new MissionCondition
          {
            ConditionType = cDto.condition_type,
            ConditionValue = cDto.condition_value,
            MapId = cDto.map_id,
            GameType = cDto.game_type
          });
        }
      }

      var rewards = new List<MissionReward>();
      if (dto.mission_reward != null)
      {
        foreach (var rDto in dto.mission_reward)
        {
          rewards.Add(new MissionReward
          {
            RewardType = rDto.reward_type,
            RewardValue = rDto.reward_value,
            ItemKey = rDto.item_key,
            ShopId = rDto.shop_id,
            PeriodType = rDto.period_type,
            Period = rDto.period,
            Color = rDto.color,
            EffectId = rDto.effect_id
          });
        }
      }

      return new MissionEntry
      {
        Id = dto.id,
        NameKey = dto.name_key,
        Conditions = conditions,
        Rewards = rewards
      };
    }

    private static MissionEntry MapMissionEntry(MissionInfoDailyPveMissionDto dto)
    {
      var conditions = new List<MissionCondition>();
      if (dto.mission_condition != null)
      {
        foreach (var cDto in dto.mission_condition)
        {
          conditions.Add(new MissionCondition
          {
            ConditionType = cDto.condition_type,
            ConditionValue = cDto.condition_value,
            MapId = cDto.map_id,
            GameType = cDto.game_type
          });
        }
      }

      var rewards = new List<MissionReward>();
      if (dto.mission_reward != null)
      {
        foreach (var rDto in dto.mission_reward)
        {
          rewards.Add(new MissionReward
          {
            RewardType = rDto.reward_type,
            RewardValue = rDto.reward_value,
            ItemKey = rDto.item_key,
            ShopId = rDto.shop_id,
            PeriodType = rDto.period_type,
            Period = rDto.period,
            Color = rDto.color,
            EffectId = rDto.effect_id
          });
        }
      }

      return new MissionEntry
      {
        Id = dto.id,
        NameKey = dto.name_key,
        Conditions = conditions,
        Rewards = rewards
      };
    }

    #endregion

    #region ArcadeReward

    public ArcadeRewardInfo LoadArcadeRewardInfo()
    {
      var dto = Deserialize<ArcadeRewardInfoDto>("xml/arcade_reward.x7");
      if (dto == null)
        return new ArcadeRewardInfo();

      var grades = new List<ArcadeRewardGrade>();
      if (dto.arcade_reward_grade != null)
      {
        foreach (var gDto in dto.arcade_reward_grade)
        {
          grades.Add(new ArcadeRewardGrade
          {
            Grade = gDto.grade,
            NameKey = gDto.name_key,
            MinScore = gDto.min_score,
            MaxScore = gDto.max_score
          });
        }
      }

      var items = new List<ArcadeRewardItem>();
      if (dto.arcade_reward_item != null)
      {
        foreach (var iDto in dto.arcade_reward_item)
        {
          items.Add(new ArcadeRewardItem
          {
            MapId = iDto.map_id,
            Difficulty = iDto.difficulty,
            Grade = iDto.grade,
            Category = iDto.category,
            SubCategory = iDto.sub_category,
            ItemNumber = iDto.item_number,
            ProductNumber = iDto.product_number,
            Probability = iDto.probability,
            MinScore = iDto.min_score,
            MaxScore = iDto.max_score
          });
        }
      }

      return new ArcadeRewardInfo { Grades = grades, Items = items };
    }

    #endregion

    #region ArcadeItem

    public ArcadeItemInfo LoadArcadeItemInfo()
    {
      var dto = Deserialize<ArcadeItemInfoDto>("xml/arcade_item.x7");
      if (dto?.arcade_item_effect == null)
        return new ArcadeItemInfo { Effects = new Dictionary<uint, ArcadeItemEffect>() };

      var effects = new Dictionary<uint, ArcadeItemEffect>();
      foreach (var eDto in dto.arcade_item_effect)
      {
        effects[eDto.item_key] = new ArcadeItemEffect
        {
          ItemKey = eDto.item_key,
          EffectType = eDto.effect_type,
          EffectValue = eDto.effect_value,
          EffectRate = eDto.effect_rate,
          EffectTime = eDto.effect_time,
          CooldownTime = eDto.cooldown_time,
          MaxStack = eDto.max_stack
        };
      }

      return new ArcadeItemInfo { Effects = effects };
    }

    #endregion

    #region ChallengeArcade

    public ChallengeArcadeInfo LoadChallengeArcadeInfo()
    {
      var dto = Deserialize<ChallengeArcadeListDto>("xml/challenge_arcade_list.x7");
      if (dto?.list_setting == null)
        return new ChallengeArcadeInfo { Entries = new List<ChallengeArcadeEntry>() };

      var entries = new List<ChallengeArcadeEntry>();
      foreach (var sDto in dto.list_setting)
      {
        entries.Add(new ChallengeArcadeEntry
        {
          Id = sDto.id,
          NameKey = sDto.name_key,
          MapId = sDto.map_id,
          Difficulty = sDto.difficulty,
          ConditionType = sDto.condition?.condition_type ?? 0,
          ConditionValue = sDto.condition?.condition_value ?? 0,
          ExpReward = sDto.reward?.exp ?? 0,
          PenReward = sDto.reward?.pen ?? 0,
          ItemKey = sDto.reward?.item_key ?? 0,
          ShopId = sDto.reward?.shop_id ?? 0,
          PeriodType = sDto.reward?.period_type,
          Period = sDto.reward?.period ?? 0,
          Color = sDto.reward?.color ?? 0,
          EffectId = sDto.reward?.effect_id ?? 0
        });
      }

      return new ChallengeArcadeInfo { Entries = entries };
    }

    #endregion

    #region TaskList

    public TaskListInfo LoadTaskListInfo()
    {
      var dto = Deserialize<TaskListInfoDto>("xml/_eu_task_list.x7");
      if (dto == null)
        return new TaskListInfo();

      return new TaskListInfo
      {
        CompulsoryTasks = MapCompulsoryTaskList(dto.compulsory_task),
        WeeklyTasks = MapWeeklyTaskList(dto.weekly_task),
        OptionalTasks = MapOptionalTaskList(dto.optional_task)
      };
    }

    private static IReadOnlyList<TaskEntry> MapBaseTaskSettings(TaskListInfoBaseSettingDto[] settings)
    {
      if (settings == null)
        return new List<TaskEntry>();

      var entries = new List<TaskEntry>();
      foreach (var bsDto in settings)
      {
        var levelSettings = new List<TaskLevelSetting>();
        if (bsDto.level_setting != null)
        {
          foreach (var lsDto in bsDto.level_setting)
          {
            levelSettings.Add(new TaskLevelSetting
            {
              Id = lsDto.id,
              Level = lsDto.level,
              ChanceValue = lsDto.chance_value,
              AddChanceValue = lsDto.add_chance_value,
              AddChanceLimitLevel = lsDto.add_chan_limit_lv,
              GamePlayTimeSeconds = lsDto.complet_condition?.game_play_ts?.value ?? 0,
              GoalOfMatch = lsDto.complet_condition?.goal_of_match?.value ?? 0,
              Repetition = lsDto.complet_condition?.repetetion?.value ?? 0,
              CheckerType = lsDto.complet_condition?.checker_type?.value,
              CheckerData = lsDto.complet_condition?.checker_type?.data,
              PenReward = lsDto.reward?.pen?.value ?? 0,
              ExPenReward = lsDto.reward?.ex_pen?.value ?? 0
            });
          }
        }

        entries.Add(new TaskEntry
        {
          NameKey = bsDto.name_key,
          ModeType = bsDto.mode_type,
          Category = bsDto.category,
          Name = bsDto.name,
          LevelSettings = levelSettings
        });
      }

      return entries;
    }

    private static IReadOnlyList<TaskEntry> MapCompulsoryTaskList(TaskListInfoCompulsoryTaskDto dto)
    {
      return MapBaseTaskSettings(dto?.base_setting);
    }

    private static IReadOnlyList<TaskEntry> MapWeeklyTaskList(TaskListInfoWeeklyTaskDto dto)
    {
      return MapBaseTaskSettings(dto?.base_setting);
    }

    private static IReadOnlyList<TaskEntry> MapOptionalTaskList(TaskListInfoOptionalTaskDto dto)
    {
      return MapBaseTaskSettings(dto?.base_setting);
    }

    #endregion

    #region PromotionInfo

    public PromotionInfo LoadPromotionInfo()
    {
      var dto = Deserialize<PromotionInfoDto>("xml/_eu_promotion_info.x7");
      if (dto == null)
        return new PromotionInfo();

      var eventInfos = new List<PromotionEventInfo>();
      if (dto.event_system?.event_info != null)
      {
        foreach (var eDto in dto.event_system.event_info)
        {
          eventInfos.Add(new PromotionEventInfo
          {
            EventType = eDto.event_type,
            Active = eDto.active,
            EventTitle = eDto.event_title,
            RewardType = eDto.reward_type,
            MinPlayer = eDto.min_player,
            MinTime = eDto.min_time,
            ChannelId = eDto.channel_id,
            MapId = eDto.map_id,
            GameMode = eDto.game_mode,
            MinScore = eDto.min_score,
            GiftType = eDto.gift_type,
            ItemKey = eDto.item_key,
            ShopId = eDto.shop_id,
            PeriodType = eDto.period_type,
            Period = eDto.period,
            Color = eDto.color,
            EffectId = eDto.effect_id,
            Probability = eDto.prob,
            RewardItemLimitCount = eDto.reward_item_limit_cnt
          });
        }
      }

      var attendance = new List<PromotionAttendanceDay>();
      if (dto.daily_attendance?.daily_item_info != null)
      {
        foreach (var aDto in dto.daily_attendance.daily_item_info)
        {
          attendance.Add(new PromotionAttendanceDay
          {
            ItemIndex = aDto.item_index,
            UserType = aDto.user_type,
            Year = aDto.year,
            Week = aDto.week,
            DayOfWeek = aDto.day_of_week,
            ItemKey = aDto.item_key,
            ShopId = aDto.shop_id,
            PeriodType = aDto.period_type,
            Period = aDto.period,
            Color = aDto.color,
            EffectId = aDto.effect_id
          });
        }
      }

      var dailyGiftRequitals = MapRequitals(dto.daily_gift?.requital);
      var dailyPlayTimeRequitals = MapRequitals(dto.daily_play_time?.requital);

      return new PromotionInfo
      {
        RouletteActive = dto.roulette_machine?.active ?? false,
        RouletteUseItemKey = dto.roulette_machine?.use_item_key ?? 0,
        RouletteUseItemCount = dto.roulette_machine?.use_item_cnt ?? 0,
        EventInfos = eventInfos,
        AttendanceDays = attendance,
        DailyGiftActive = dto.daily_gift?.active ?? false,
        DailyGiftRequitals = dailyGiftRequitals,
        DailyPlayTimeActive = dto.daily_play_time?.active ?? false,
        DailyPlayTimeRequitals = dailyPlayTimeRequitals
      };
    }

    private static IReadOnlyList<PromotionRequital> MapRequitals(PromotionInfoRequitalDto[] dtos)
    {
      if (dtos == null)
        return new List<PromotionRequital>();

      var result = new List<PromotionRequital>();
      foreach (var dto in dtos)
      {
        result.Add(new PromotionRequital
        {
          Key = dto.key,
          GiftType = dto.gift_type,
          GiftValue = dto.gift_value,
          ItemKey = dto.item_key,
          ShopId = dto.shop_id,
          PeriodType = dto.period_type,
          Period = dto.period,
          Color = dto.color,
          EffectId = dto.effect_id,
          Probability = dto.prob
        });
      }

      return result;
    }

    #endregion

    #region MakeCharacterInfo

    public MakeCharacterInfo LoadMakeCharacterInfo()
    {
      var dto = Deserialize<MakeCharacterInfoDto>("xml/_eu_make_character_info.x7");
      if (dto?.character == null)
        return new MakeCharacterInfo();

      var ch = dto.character;

      MakeCharacterCostume MapCostume(MakeCharacterInfoCostumeGenderDto d)
      {
        if (d == null) return null;
        return new MakeCharacterCostume
        {
          HairItemId = d.hair?.itemid ?? 0,
          HairVariation = d.hair?.variation ?? 0,
          FaceItemId = d.face?.itemid ?? 0,
          FaceVariation = d.face?.variation ?? 0,
          CoatItemId = d.coat?.itemid ?? 0,
          CoatVariation = d.coat?.variation ?? 0,
          PantsItemId = d.pants?.itemid ?? 0,
          PantsVariation = d.pants?.variation ?? 0,
          GlovesItemId = d.gloves?.itemid ?? 0,
          GlovesVariation = d.gloves?.variation ?? 0,
          ShoesItemId = d.shoes?.itemid ?? 0,
          ShoesVariation = d.shoes?.variation ?? 0
        };
      }

      var defaultGender = new MakeCharacterGender();
      if (ch.@default?.costume != null)
      {
        defaultGender.MaleCostume = MapCostume(ch.@default.costume.male);
        defaultGender.FemaleCostume = MapCostume(ch.@default.costume.female);
      }

      var weapons = new List<MakeCharacterWeapon>();
      if (ch.weapons?.weapon != null)
      {
        foreach (var wDto in ch.weapons.weapon)
        {
          weapons.Add(new MakeCharacterWeapon
          {
            ItemId = wDto.itemid,
            ShopId = wDto.shopid,
            PeriodType = wDto.periodtype,
            Period = wDto.period,
            Color = wDto.color,
            EffectId = wDto.effectid,
            Slot = wDto.slot,
            Equip = wDto.equip
          });
        }
      }

      var skills = new List<MakeCharacterSkill>();
      if (ch.skills?.skill != null)
      {
        foreach (var sDto in ch.skills.skill)
        {
          skills.Add(new MakeCharacterSkill
          {
            ItemId = sDto.itemid,
            ShopId = sDto.shopid,
            PeriodType = sDto.periodtype,
            Period = sDto.period,
            Color = sDto.color,
            EffectId = sDto.effectid,
            Equip = sDto.equip
          });
        }
      }

      return new MakeCharacterInfo
      {
        MaleCostumes = new List<MakeCharacterItem>(),
        FemaleCostumes = new List<MakeCharacterItem>(),
        Weapons = weapons,
        Skills = skills,
        DefaultGender = defaultGender
      };
    }

    #endregion

    #region SupportItem

    public SupportItemInfo LoadSupportItemInfo()
    {
      var dto = Deserialize<SupportItemInfoDto>("xml/support_item.x7");
      if (dto?.item == null)
        return new SupportItemInfo { Items = new List<SupportItemEntry>() };

      var items = new List<SupportItemEntry>();
      foreach (var iDto in dto.item)
      {
        items.Add(new SupportItemEntry
        {
          Category = iDto.category,
          SubCategory = iDto.sub_category,
          Number = iDto.number,
          Product = iDto.product,
          Slot = iDto.slot
        });
      }

      return new SupportItemInfo { Items = items };
    }

    #endregion
  }
}