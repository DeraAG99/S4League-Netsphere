# S4League-Netsphere

S4 League Private Server Emulator

## Status

- Build: `dotnet build src/GameServer/GameServer.csproj -c LatestOld_Debug`
- Target: `netcoreapp2.0`
- Database: MySQL

## Bug Fixes

### Critical

| File | Issue | Fix |
|------|-------|-----|
| `Player.cs` | Thread-unsafe `Dispose()` — race condition on `_disposed` field | Atomic `Interlocked.CompareExchange` pattern |
| `Inventory.cs:386` | Capsule items lost on relog — period not accumulated | Grace period + `DaysLeft` accumulation |
| `ShopPrice.cs` | Crash when `CapsulePrice` is null | Added fallback price check |
| `DbUtil.cs:262` | SQL injection via string concatenation | Parameterized queries |
| `AdminCommands.cs:148,218` | Null reference on empty ban list | Null checks before access |
| `AuthService.cs:46` | Session crash on duplicate login | Proper session cleanup |
| `PeerService.cs:40` | UDP handler not initialized | Init check before access |

### Medium

| File | Issue | Fix |
|------|-------|-----|
| `BanCommands.cs:56` | Ban duration overflow | Proper `TimeSpan` handling |
| `ClanCommands.cs:65` | Null ref when creating clan | Null-safe member check |
| `AdminCommands.cs:380-382` | Wrong parameter type in admin command | Fixed type casting |
| `TeamManager.cs:87` | Team assignment crash on full teams | Overflow protection |
| `PlayerRecord.cs:28` | EXP calculation not using XML data | Rewritten to use `experience_bonus.x7` |

### Networking

| File | Issue | Fix |
|------|-------|-----|
| `ProudSession.cs` | UDP fallback to TCP not working | Proper fallback logic |
| `UdpHandler.cs` | SessionId collision | Collision guard added |

## New Features

### EXP / PEN / Master Level System

- `PlayerRecord.GetExpGain()` — rewritten to use `experience_bonus.x7` XML per game mode
- `PlayerRecord.GetPenGain()` — rewritten to use `point_bonus.x7` XML per game mode + level bonuses
- `Player.GainExp()` — overflow XP routes to `MasterExperience`/`MasterLevel` at max level
- `MasterExperience` — 50-level master XP table from `master_experience.x7`

### Random Shop (Fumbishop)

- `RandomShopEntry.cs`, `RandomShopResourceDto.cs` — weighted random pool system
- `data/xml/RandomShop.xml` — configurable shop pools
- `ShopService.cs` — random shop handler

### Card System

- `CardSystemInfo.cs`, `CardSystemResourceDto.cs` — card collection with seasons, gamble, rewards
- `player_cardcollections` table in DB
- `CardGambleHandler` in `ShopService.cs`

### XML Resource Loading System

The server now loads client XML resources at startup for data-driven configuration.

**35 XML files loaded**, each with: DTO → Runtime Model → Loader → Cache

| Category | Files Loaded |
|----------|-------------|
| Game Balance | `experience_bonus.x7`, `point_bonus.x7`, `master_experience.x7`, `experience.x7` |
| Burning Time | `burning_time.x7`, `burning_time_pve.x7` |
| Equipment | `equip_limit.x7`, `item_grade.x7`, `combine_element_info.x7`, `decomposition_element_info.x7` |
| Enchanting | `enchant_data.x7`, `enchant_list.x7`, `enchant_extractkey.x7`, `esper_enchant_price.x7` |
| Room Config | `room_option.x7` |
| Siege Mode | `seize_mode_newinfo.x7`, `stadium_info.x7` |
| Missions | `mission.x7` |
| Arcade | `arcade_item.x7`, `arcade_reward.x7`, `challenge_arcade_list.x7` |
| Crafting | `_eu_combination_info.x7`, `_eu_decomposition_info.x7` |
| Tasks | `_eu_task_list.x7` |
| Promotions | `_eu_promotion_info.x7` |
| Character | `_eu_make_character_info.x7`, `support_item.x7` |
| Card System | `_eu_card_system_info.x7` |
| Items | `item.x7`, `iteminfo.x7`, `item_effect.x7`, `default_item.x7`, `constant_info.x7` |
| Maps | `map.x7`, `monster_status.x7` |
| Shop | `RandomShop.xml`, `ItemBag.xml` |

### Key Runtime Models

| Model | Purpose |
|-------|---------|
| `ExperienceBonusConfig` | EXP formulas per game mode |
| `PointBonusConfig` | PEN formulas per game mode + level bonuses |
| `MasterExperience` | 50-level master XP requirements |
| `BurningTimeInfo` | Stat multipliers per level/mode |
| `EquipLimitInfo` | Weapon loadout restrictions per mode |
| `EnchantInfo` | Enchant prices, mastery, probabilities |
| `DecompositionInfo` | Item decomposition methods, costs, prohibition list |
| `CombinationInfo` | Item crafting components, enchant options |
| `MissionInfo` | Daily PVP/PVE missions with conditions and rewards |
| `ArcadeRewardInfo` | Arcade grades and item drops per map/difficulty |
| `ArcadeItemInfo` | Arcade power-up effects, cooldowns, stacks |
| `ChallengeArcadeInfo` | Challenge missions with conditions and rewards |
| `TaskListInfo` | Tutorial tasks (compulsory/weekly/optional) |
| `PromotionInfo` | Events, roulette, attendance, daily gifts |
| `MakeCharacterInfo` | Character creation defaults (costumes, weapons, skills) |
| `SupportItemInfo` | First-login starter items |
| `CardSystemInfo` | Card seasons, gamble, rewards |
| `RandomShopPool` | Fumbishop item pools with rates |

## Build

```bash
dotnet build src/GameServer/GameServer.csproj -c LatestOld_Debug
```

## Configuration

`GameServer.hjson` — server config (IP, ports, database, game settings)

## Database

- Auth: `s4casey` (account data)
- Game: `s4casey` (player data, inventory, etc.)
- Schema: `netsphere.sql`
