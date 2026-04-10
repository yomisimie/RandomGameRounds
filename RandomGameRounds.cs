using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Timers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RandomGameRounds;
public class RandomGameRounds : BasePlugin
{
    private const int DefaultGravity = 800;
    private const int LowGravityValue = 350;
    private const int HighGravityValue = 1400;
    private const int DefaultGrenadeLimit = 4;
    private const int NadeFrenzyGrenadeLimit = 6;
    private const string NoScopeAwpEffectName = "AWP";
    private const string PistolsOnlyEffectName = "Pistols Only";
    private const string HeadshotsOnlyEffectName = "Headshots Only";
    private const string OneInTheChamberEffectName = "One in the Chamber";
    private const string InvisibleRadarEffectName = "Invisible Radar";
    private const string ScoutzKnivezEffectName = "ScoutzKnivez";
    private const string RandomWeaponsEffectName = "Random Weapons";
    private const string KnifeOnlyEffectName = "Knife Only";
    private const string GodsOfThunderEffectName = "Gods of Thunder";
    private const string LowGravityEffectName = "Low Gravity";
    private const string HighGravityEffectName = "High Gravity";
    private const string Hp35EffectName = "35 HP";
    private const string Hp150EffectName = "150 HP";
    private const string NoArmorEffectName = "No Armor";
    private const string NadeFrenzyEffectName = "Nade Frenzy";
    private const string ShotgunsOnlyEffectName = "Shotguns Only";
    private const string SmgRushEffectName = "SMG Rush";
    private const string HeavyOnlyEffectName = "Heavy Only";
    private const string PistolRouletteEffectName = "Pistol Roulette";
    private const string DeagleDuelEffectName = "Deagle Duel";
    private const string AutoSniperEffectName = "Auto-Sniper Mayhem";
    private const string LowAmmoEffectName = "Low Ammo";
    private const string AbundentAmmoEffectName = "Abundent Ammo";
    private const string SlowMovementEffectName = "Slow Movement";
    private const string VampireEffectName = "Vampire";
    private const string OneTapEffectName = "One Tap";
    private const string ClumsyEffectName = "Clumsy";
    private const string HealingC4EffectName = "Healing C4";
    private const string NuclearC4EffectName = "Nuclear Proximity";
    private const string SilentHillEffectName = "Silent Hill";
    private const int ClumsyDropChancePercent = 8;
    private const float DefaultMovementMultiplier = 1.0f;
    private const float SlowMovementMultiplier = 0.45f;
    private const float DefaultAccelerate = 5.5f;
    private const float SlowAccelerate = 3.0f;
    private const float DefaultAirAccelerate = 12.0f;
    private const float SlowAirAccelerate = 8.0f;
    private const int DefaultRoundHp = 100;
    private const int DefaultMaxSpeed = 315;
    private const int SlowMovementSpeed = 130;
    private const int VampireHealAmount = 30;
    private const int VampireHealMax = 150;

    private static readonly HashSet<string> ActiveEffects = new();
    private static bool _isWarmup;
    private static bool _roundEffectApplied;
    private static bool _headshotOnlyActive;
    private static bool _vampireActive;
    private static bool _oneTapActive;
    private static bool _clumsyActive;

    private CounterStrikeSharp.API.Modules.Timers.Timer? _nuclearShakeTimer;

    private static readonly string[] LoadoutEffects =
    {
        KnifeOnlyEffectName,
        NoScopeAwpEffectName,
        PistolsOnlyEffectName,
        OneInTheChamberEffectName,
        RandomWeaponsEffectName,
        ScoutzKnivezEffectName,
        ShotgunsOnlyEffectName,
        SmgRushEffectName,
        HeavyOnlyEffectName,
        PistolRouletteEffectName,
        DeagleDuelEffectName,
        AutoSniperEffectName,
        GodsOfThunderEffectName,
    };

    private static readonly string[] ModifierEffects =
    {
        LowGravityEffectName,
        HighGravityEffectName,
        HeadshotsOnlyEffectName,
        InvisibleRadarEffectName,
        Hp35EffectName,
        Hp150EffectName,
        NoArmorEffectName,
        NadeFrenzyEffectName,
        LowAmmoEffectName,
        AbundentAmmoEffectName,
        SlowMovementEffectName,
        VampireEffectName,
        OneTapEffectName,
        ClumsyEffectName,
        HealingC4EffectName,
        NuclearC4EffectName
    };

    private static readonly string[] RandomPrimaryWeaponPool =
    {
        "weapon_mac10",
        "weapon_mp9",
        "weapon_mp7",
        "weapon_mp5sd",
        "weapon_ump45",
        "weapon_p90",
        "weapon_bizon",
        "weapon_galil",
        "weapon_famas",
        "weapon_ak47",
        "weapon_m4a1",
        "weapon_m4a1_silencer",
        "weapon_sg556",
        "weapon_aug",
    };

    private static readonly string[] RandomPistolWeaponPool =
    {
        "weapon_glock",
        "weapon_hkp2000",
        "weapon_usp_silencer",
        "weapon_elite",
        "weapon_p250",
        "weapon_tec9",
        "weapon_cz75a",
        "weapon_fiveseven",
        "weapon_deagle",
        "weapon_revolver"
    };

    private static readonly string[] ShotgunWeaponPool =
    {
        "weapon_nova",
        "weapon_xm1014",
        "weapon_mag7",
        "weapon_sawedoff"
    };

    private static readonly string[] SmgWeaponPool =
    {
        "weapon_mac10",
        "weapon_mp9",
        "weapon_mp7",
        "weapon_mp5sd",
        "weapon_ump45",
        "weapon_p90",
        "weapon_bizon",
    };

    private static readonly string[] HeavyWeaponPool =
    {
        "weapon_negev",
        "weapon_m249"
    };

    private static readonly string[] AutoSniperPool =
    {
        "weapon_scar20",
        "weapon_g3sg1"
    };

    private static readonly string[] NadeFrenzyGrenades =
    {
        "weapon_hegrenade",
        "weapon_flashbang",
        "weapon_smokegrenade",
        "weapon_decoy",
        "weapon_molotov",
        "weapon_incgrenade"
    };

    private static readonly string[] ClumsyDropMessages =
    {
        "Oops!",
        "Butterfingers!",
        "Not again...",
        "Slippery hands!",
        "Dropped it!",
        "Fumble!",
        "Clumsy!",
        "Pick that up!",
        "Whoops!",
        "Grip it properly!",
    };

    public override string ModuleName => "Random Game Rounds";

    public override string ModuleVersion => "0.0.1";

    public override void Load(bool hotReload)
    {
        Console.WriteLine("Random Game Rounds loaded.");

        ApplyServerDefaults();
        AddTimer(2.0f, () =>
        {
            ApplyServerDefaults();
            Console.WriteLine("[Round Effect] Re-applied server defaults after startup delay.");
        });

        RegisterListener<Listeners.OnMapStart>(mapName =>
        {
            ApplyServerDefaults();
            AddTimer(2.0f, () =>
            {
                ApplyServerDefaults();
                Console.WriteLine("[Round Effect] Re-applied server defaults after map-start delay.");
            });
        });

        AddTimer(0.2f, EnforceOneInTheChamberAmmoCap, TimerFlags.REPEAT);
        AddTimer(5.0f, ReplenishNadeFrenzyGrenades, TimerFlags.REPEAT);

        AddCommand("css_effect_lowgravity",  "Force Low Gravity effect for current round",            (_, cmd) => ForceEffects(cmd, LowGravityEffectName));
        AddCommand("css_effect_highgravity", "Force High Gravity effect for current round",           (_, cmd) => ForceEffects(cmd, HighGravityEffectName));
        AddCommand("css_effect_knife",       "Force Knife Only loadout effect for current round",    (_, cmd) => ForceEffects(cmd, KnifeOnlyEffectName));
        AddCommand("css_effect_awp",         "Force AWP loadout effect for current round",           (_, cmd) => ForceEffects(cmd, NoScopeAwpEffectName));
        AddCommand("css_effect_noscope",     "[Alias] Force AWP loadout effect for current round",   (_, cmd) => ForceEffects(cmd, NoScopeAwpEffectName));
        AddCommand("css_effect_pistolhs",    "Force Pistols Only + Headshots Only for current round",(_, cmd) => ForceEffects(cmd, PistolsOnlyEffectName, HeadshotsOnlyEffectName));
        AddCommand("css_effect_pistols",     "Force Pistols Only loadout effect for current round",  (_, cmd) => ForceEffects(cmd, PistolsOnlyEffectName));
        AddCommand("css_effect_chamber",     "Force One in the Chamber effect for current round",    (_, cmd) => ForceEffects(cmd, OneInTheChamberEffectName));
        AddCommand("css_effect_random",      "Force Random Weapons loadout effect for current round",(_, cmd) => ForceEffects(cmd, RandomWeaponsEffectName));
        AddCommand("css_effect_radaroff",    "Force Invisible Radar modifier for current round",     (_, cmd) => ForceEffects(cmd, InvisibleRadarEffectName));
        AddCommand("css_effect_nades",       "Force Nade Frenzy modifier for current round",         (_, cmd) => ForceEffects(cmd, NadeFrenzyEffectName));
        AddCommand("css_effect_noarmor",     "Force No Armor modifier for current round",            (_, cmd) => ForceEffects(cmd, NoArmorEffectName));
        AddCommand("css_effect_scoutz",      "Force ScoutzKnivez effect for current round",          (_, cmd) => ForceEffects(cmd, ScoutzKnivezEffectName));
        AddCommand("css_effect_shotguns",    "Force Shotguns Only loadout for current round",        (_, cmd) => ForceEffects(cmd, ShotgunsOnlyEffectName));
        AddCommand("css_effect_smg",         "Force SMG Rush loadout for current round",             (_, cmd) => ForceEffects(cmd, SmgRushEffectName));
        AddCommand("css_effect_heavy",       "Force Heavy Only loadout for current round",           (_, cmd) => ForceEffects(cmd, HeavyOnlyEffectName));
        AddCommand("css_effect_roulette",    "Force Pistol Roulette loadout for current round",      (_, cmd) => ForceEffects(cmd, PistolRouletteEffectName));
        AddCommand("css_effect_deagle",      "Force Deagle Duel loadout for current round",          (_, cmd) => ForceEffects(cmd, DeagleDuelEffectName));
        AddCommand("css_effect_autosniper",  "Force Auto-Sniper Mayhem loadout for current round",   (_, cmd) => ForceEffects(cmd, AutoSniperEffectName));
        AddCommand("css_effect_lowammo",     "Force Low Ammo modifier for current round",            (_, cmd) => ForceEffects(cmd, LowAmmoEffectName));
        AddCommand("css_effect_moreammo",    "Force Abundent Ammo modifier for current round",        (_, cmd) => ForceEffects(cmd, AbundentAmmoEffectName));
        AddCommand("css_effect_slow",        "Force Slow Movement modifier for current round",        (_, cmd) => ForceEffects(cmd, SlowMovementEffectName));
        AddCommand("css_effect_vampire",     "Force Vampire modifier for current round",              (_, cmd) => ForceEffects(cmd, VampireEffectName));
        AddCommand("css_effect_onetap",      "Force One Tap modifier for current round",              (_, cmd) => ForceEffects(cmd, OneTapEffectName));
        AddCommand("css_effect_clumsy",      "Force Clumsy modifier for current round",               (_, cmd) => ForceEffects(cmd, ClumsyEffectName));
        AddCommand("css_effect_healc4",      "Force Healing C4 modifier for current round",           (_, cmd) => ForceEffects(cmd, HealingC4EffectName));
        AddCommand("css_effect_nuclearc4",   "Force Nuclear proximity modifier for current round",    (_, cmd) => ForceEffects(cmd, NuclearC4EffectName));
        AddCommand("css_effect_thunder",     "Force Gods of Thunder modifier for current round",      (_, cmd) => ForceEffects(cmd, GodsOfThunderEffectName));
        AddCommand("css_effect_fog",         "Force Silent Hill modifier for current round",          (_, cmd) => ForceEffects(cmd, SilentHillEffectName));
        AddCommand("css_effect_rules",       "Show effect compatibility rules",                       ShowEffectRulesCommand);
        AddCommand("css_effect_clear",       "Clear active forced effect state",                      ClearEffectCommand);

        RegisterListener<Listeners.OnPlayerButtonsChanged>((player, pressed, released) =>
        {
            if (!player.IsValid || player.PawnIsAlive != true)
            {
                return;
            }

            if (ActiveEffects.Contains(NoScopeAwpEffectName) && (pressed & PlayerButtons.Zoom) != 0)
            {
                ApplyNoScopeAwpToPlayer(player);
                QueueAmmoEnforcementAfterGive(player);
            }
        });

        RegisterListener<Listeners.OnPlayerTakeDamagePre>((player, damageInfo) =>
        {
            if (player == null || !player.IsValid) return HookResult.Continue;
            if (ActiveEffects.Contains(AbundentAmmoEffectName)) 
            {
                var weaponServices = player.WeaponServices;
                
                // If no weapon services, just exit this block and continue the hook
                if (weaponServices != null)
                {
                    foreach (var weaponHandle in weaponServices.MyWeapons)
                    {
                        var weapon = weaponHandle.Value;
                        if (weapon == null || !weapon.IsValid) continue;
        
                        try 
                        {
                            var reserveAmmo = weapon.ReserveAmmo;
                            for (var index = 0; index < reserveAmmo.Length; index++)
                            {
                                reserveAmmo[index]++;
                            }
                            Console.WriteLine($"[RandomGameRounds] Ammo refilled for pawn: {player.Index}");
        
                            // Inform the engine the state has changed
                            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_pReserveAmmo");
                        }
                        catch 
                        {
                            // Silently catch potential errors with non-gun entities
                        }
                    }
                }
            }
        
            if (_oneTapActive || ActiveEffects.Contains(OneInTheChamberEffectName))
            {
                damageInfo.Damage = 1000.0f;
                return HookResult.Continue;
            }

            if (!_headshotOnlyActive)
            {
                return HookResult.Continue;
            }

            var hitGroup = damageInfo.GetHitGroup();
            if (hitGroup != HitGroup_t.HITGROUP_HEAD)
            {
                return HookResult.Handled;
            }

            return HookResult.Continue;
        });

        RegisterEventHandler<EventWeaponFire>((@event, info) =>
        {
            // Check if the effect is active and the weapon fired was a taser
            if (ActiveEffects.Contains(GodsOfThunderEffectName) && @event.Weapon == "weapon_taser")
            {
                var shooter = @event.Userid;
                
                if (shooter != null && shooter.IsValid && shooter.PawnIsAlive)
                {
                    // We get the active weapon from the player's pawn
                    var weapon = shooter.PlayerPawn.Value?.WeaponServices?.ActiveWeapon.Value;
        
                    if (weapon != null && weapon.DesignerName == "weapon_taser")
                    {
                        // Reset the 'Clip' to 1 (this is the single charge)
                        weapon.Clip1 = 1;
                        // Tell the game the ammo count has changed so the HUD updates
                        Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_iClip1");
                    }
                }
            }
        
            if (!_clumsyActive)
            {
                return HookResult.Continue;
            }

            var player = @event.Userid;
            if (player == null || !player.IsValid || player.PawnIsAlive != true)
            {
                return HookResult.Continue;
            }

            if (Random.Shared.Next(100) >= ClumsyDropChancePercent)
            {
                return HookResult.Continue;
            }

            var weaponServices = player.PlayerPawn.Value?.WeaponServices;
            var activeWeapon = weaponServices?.ActiveWeapon?.Value;
            if (activeWeapon == null || !activeWeapon.IsValid)
            {
                return HookResult.Continue;
            }

            // Don't drop knife or grenades.
            var name = activeWeapon.DesignerName ?? string.Empty;
            if (name.StartsWith("weapon_knife", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("grenade", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("molotov", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("decoy", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("flashbang", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("smoke", StringComparison.OrdinalIgnoreCase))
            {
                return HookResult.Continue;
            }

            player.DropActiveWeapon();
            player.PrintToCenter(ClumsyDropMessages[Random.Shared.Next(ClumsyDropMessages.Length)]);

            return HookResult.Continue;
        });

        RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
        {
            var player = @event.Userid;
            if (player == null || !player.IsValid)
            {
                return HookResult.Continue;
            }

            var activeLoadout = GetActiveLoadoutEffect();
            if (!string.IsNullOrEmpty(activeLoadout))
            {
                Server.NextWorldUpdate(() => ApplyLoadoutToPlayer(player, activeLoadout));
            }

            if (ActiveEffects.Contains(NoArmorEffectName))
            {
                Server.NextWorldUpdate(() => ApplyNoArmorToPlayer(player));
                Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyNoArmorToPlayer(player)));
            }

            if (ActiveEffects.Contains(LowAmmoEffectName))
            {
                Server.NextWorldUpdate(() => ApplyLowAmmoToPlayer(player));
                Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyLowAmmoToPlayer(player)));
            }

            if (ActiveEffects.Contains(OneTapEffectName))
            {
                Server.NextWorldUpdate(() => ApplyOneTapToPlayer(player));
            }

            if (ActiveEffects.Contains(OneInTheChamberEffectName))
            {
                Server.NextWorldUpdate(() => ApplyOneInTheChamberAmmoToPlayer(player));
                Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyOneInTheChamberAmmoToPlayer(player)));
            }

            if (ActiveEffects.Contains(NadeFrenzyEffectName))
            {
                Server.NextWorldUpdate(() => ApplyNadeFrenzyToPlayer(player));
                Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyNadeFrenzyToPlayer(player)));
            }

            Server.NextWorldUpdate(() => ApplyActiveMovementEffectToPlayer(player));
            Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyActiveMovementEffectToPlayer(player)));

            Server.NextWorldUpdate(() => ApplyActiveHpEffectToPlayer(player));
            Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyActiveHpEffectToPlayer(player)));

            return HookResult.Continue;
        });

        RegisterEventHandler<EventPlayerDeath>((@event, info) =>
        {
            var attacker = @event.Attacker;
            if (attacker == null || !attacker.IsValid || attacker == @event.Userid || attacker.PawnIsAlive != true)
            {
                return HookResult.Continue;
            }

            if (_vampireActive)
            {
                var pawn = attacker.PlayerPawn.Value;
                if (pawn != null && pawn.IsValid)
                {
                    pawn.Health = Math.Min(pawn.Health + VampireHealAmount, VampireHealMax);
                }
            }

            // One in the Chamber: killing rewards the attacker with 1 bullet.
            if (ActiveEffects.Contains(OneInTheChamberEffectName))
            {
                Server.NextWorldUpdate(() => ApplyOneInTheChamberAmmoToPlayer(attacker));
            }

            if (ActiveEffects.Contains(LowAmmoEffectName))
            {
                Server.NextWorldUpdate(() => RewardLowAmmoReserveOnKill(attacker));
            }

            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundPrestart>((@event, info) =>
        {
            _roundEffectApplied = false;

            if (_isWarmup)
            {
                ClearCurrentEffectState();
                Console.WriteLine("[Round Effect] Warmup detected, skipping effect selection.");
                return HookResult.Continue;
            }

            TriggerRandomRoundEffect("round_prestart");

            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundStart>((@event, info) =>
        {
            // 1. Cleanup old round state
            _nuclearShakeTimer?.Kill();
            _nuclearShakeTimer = null;
            
            // Always reset fog at round start so it doesn't bleed into non-fog rounds
            ResetMapFog();
            
            if (ActiveEffects.Contains(NoArmorEffectName))
            {
                ApplyNoArmor();
                AddTimer(0.5f, ApplyNoArmor);
            }

            if (_roundEffectApplied)
            {
                return HookResult.Continue;
            }

            if (_isWarmup)
            {
                return HookResult.Continue;
            }

            TriggerRandomRoundEffect("round_start_fallback");
            // 2. Check if Silent Hill was just picked by TriggerRandomRoundEffect
            if (ActiveEffects.Contains(SilentHillEffectName))
            {
                ApplySilentHillFog();
            }
            
            // Give the bomb after a tiny delay to ensure players have spawned
            AddTimer(0.5f, () => {
                GiveBombToRandomT();
            });
            
            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundAnnounceWarmup>((@event, info) =>
        {
            _isWarmup = true;
            _roundEffectApplied = false;
            ClearCurrentEffectState();
            Console.WriteLine("[Round Effect] Warmup started.");
            return HookResult.Continue;
        });

        RegisterEventHandler<EventRoundAnnounceMatchStart>((@event, info) =>
        {
            _isWarmup = false;

            if (!_roundEffectApplied)
            {
                TriggerRandomRoundEffect("match_start_fallback");
            }

            Console.WriteLine("[Round Effect] Warmup ended. Match started.");
            return HookResult.Continue;
        });

        RegisterEventHandler<EventBombPlanted>((@event, info) =>
        {
            ApplyPlantEffects();
            return HookResult.Continue;
        });
    }

    private void ApplySilentHillFog()
    {
        // 1. Find the camera entity (often named 'point_camera' or similar)
        var camera = Utilities.FindAllEntitiesByDesignerName<CPointCamera>("point_camera").FirstOrDefault();
    
        if (camera != null)
        {
            // 2. Apply the schema properties you found
            camera.FogEnable = true;
            camera.FogStart = 0.0f;
            camera.FogEnd = 600.0f;
            camera.FogMaxDensity = 1.0f;
            camera.FogColor = System.Drawing.Color.FromArgb(255, 180, 180, 180);
    
            // 3. Update the state
            Utilities.SetStateChanged(camera, "CPointCamera", "m_bFogEnable");
            Utilities.SetStateChanged(camera, "CPointCamera", "m_flFogEnd");
            
            Server.PrintToChatAll(" [\x02! \x01] A thick mist rolls in... Welcome to Silent Hill.");
        }
        else
        {
            // Fallback: If the map doesn't have a CPointCamera, we use the Player's camera services
            // as a backup so the effect still works on every map!
            ApplyCameraServicesFog();
        }
    }
    
    private void ResetMapFog()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            var pawn = player.PlayerPawn.Value;
            var cameraServices = pawn?.CameraServices;
            if (cameraServices != null)
            {
                cameraServices.FogEnable = false;
                Utilities.SetStateChanged(pawn!, "CBasePlayerPawn", "m_pCameraServices");
            }
        }
    }

    private void ApplyPlantEffects()
    {
        // Heal Ts
        if (ActiveEffects.Contains(HealingC4EffectName)) {
            foreach (var player in Utilities.GetPlayers().Where(p => p.TeamNum == 2 && p.PawnIsAlive))
            {
                player.PawnHealth = 100;
                Utilities.SetStateChanged(player.PlayerPawn.Value!, "CBaseEntity", "m_iHealth");
            }
        }
        // Nuclear Proximity for all
        if (ActiveEffects.Contains(NuclearC4EffectName)) {
            _nuclearShakeTimer = AddTimer(0.2f, () => 
            {
                foreach (var p in Utilities.GetPlayers().Where(p => p.PawnIsAlive))
                {
                    var pawn = p.PlayerPawn.Value;
                    
                    // Access the CameraServices component
                    var cameraServices = pawn?.CameraServices;
                    if (cameraServices == null) continue;
            
                    Random rnd = new();
                    float intensity = 1.5f;
            
                    // Use the property name from your schema: CsViewPunchAngle
                    cameraServices.CsViewPunchAngle.X += (float)(rnd.NextDouble() * intensity * 2 - intensity);
                    cameraServices.CsViewPunchAngle.Y += (float)(rnd.NextDouble() * intensity * 2 - intensity);
                    
                    // Inform the engine that the CameraServices state has changed
                    Utilities.SetStateChanged(pawn!, "CBasePlayerPawn", "m_pCameraServices");
                }
            }, TimerFlags.REPEAT);
        }
    }

    private void GiveBombToRandomT()
    {
        // Get all valid players on the Terrorist side (TeamNum 2)
        var terrorists = Utilities.GetPlayers().Where(p => 
            p.IsValid && 
            p.PawnIsAlive && 
            p.TeamNum == 2 && // 2 is Terrorist, 3 is CT
            !p.IsBot
        ).ToList();
    
        if (terrorists.Count > 0)
        {
            // Pick a random player from the list
            Random rnd = new Random();
            var luckyPlayer = terrorists[rnd.Next(terrorists.Count)];
    
            // Give the C4
            luckyPlayer.GiveNamedItem("weapon_c4");
        }
    }

    private static void TriggerRandomRoundEffect(string source)
    {
        var selectedEffects = BuildRandomEffectSet();

        ClearCurrentEffectState();
        foreach (var effect in selectedEffects)
        {
            ActiveEffects.Add(effect);
        }

        _roundEffectApplied = true;
        Console.WriteLine($"[Round Effect] ({source}) Selected effects: {string.Join(", ", selectedEffects)}");
        Server.PrintToChatAll($" \x04[Round Effects]\x01: \x05{string.Join("\x01 | \x05", selectedEffects)}");
        foreach (var p in Utilities.GetPlayers())
        {
            p.PrintToCenter($"{string.Join(" | ", selectedEffects)}");
        }

        foreach (var effect in selectedEffects)
        {
            ApplyRoundEffect(effect);
        }

        if (selectedEffects.Contains(NoArmorEffectName))
        {
            ApplyNoArmor();
            Server.NextWorldUpdate(() => ApplyNoArmor());
            Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyNoArmor()));
        }
        else
        {
            ApplyArmorToAllPlayers();
        }

        if (selectedEffects.Contains(LowAmmoEffectName))
        {
            ApplyLowAmmo();
            Server.NextWorldUpdate(() => ApplyLowAmmo());
            Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyLowAmmo()));
        }

        if (selectedEffects.Contains(NadeFrenzyEffectName))
        {
            ApplyNadeFrenzy();
            Server.NextWorldUpdate(() => ApplyNadeFrenzy());
            Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyNadeFrenzy()));
        }

        if (selectedEffects.Contains(OneInTheChamberEffectName))
        {
            ApplyOneInTheChamberAmmo();
            Server.NextWorldUpdate(() => ApplyOneInTheChamberAmmo());
            Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyOneInTheChamberAmmo()));
        }

        if (selectedEffects.Contains(GodsOfThunderEffectName))
        {
            foreach (var player in Utilities.GetPlayers())
            {
                player.GiveNamedItem("weapon_taser");
            }
        }

        ApplyActiveHpEffect();
        Server.NextWorldUpdate(() => ApplyActiveHpEffect());
        Server.NextWorldUpdate(() => Server.NextWorldUpdate(() => ApplyActiveHpEffect()));
    }

    private static IReadOnlyList<string> BuildRandomEffectSet()
    {
        const int targetCount = 3; // 1 loadout + 2 modifiers
        const int maxAttempts = 24;

        IReadOnlyList<string>? bestAttempt = null;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var selectedEffects = new List<string>();

            var loadoutEffect = LoadoutEffects[Random.Shared.Next(LoadoutEffects.Length)];
            selectedEffects.Add(loadoutEffect);

            var shuffledModifiers = ModifierEffects
                .OrderBy(_ => Random.Shared.Next())
                .ToList();

            foreach (var modifier in shuffledModifiers)
            {
                if (!CanWorkTogether(selectedEffects, modifier))
                {
                    continue;
                }

                selectedEffects.Add(modifier);
                if (selectedEffects.Count >= targetCount)
                {
                    return selectedEffects;
                }
            }

            if (bestAttempt == null || selectedEffects.Count > bestAttempt.Count)
            {
                bestAttempt = selectedEffects;
            }
        }

        return bestAttempt ?? new[] { LoadoutEffects[Random.Shared.Next(LoadoutEffects.Length)] };
    }

    // Symmetric pairs of effects that cannot be combined. Order within each tuple does not matter.
    private static readonly HashSet<(string, string)> IncompatiblePairs = new()
    {
        (HeadshotsOnlyEffectName, OneInTheChamberEffectName),
        (HeadshotsOnlyEffectName, LowAmmoEffectName),
        (HeadshotsOnlyEffectName, KnifeOnlyEffectName),
        (HeadshotsOnlyEffectName, OneTapEffectName),
        (Hp35EffectName,          Hp150EffectName),
        (LowGravityEffectName,    HighGravityEffectName),
        (LowAmmoEffectName,       KnifeOnlyEffectName),
        (ClumsyEffectName,        KnifeOnlyEffectName),
        (ClumsyEffectName,        OneTapEffectName),
        (ClumsyEffectName,        OneInTheChamberEffectName),
        (AbundentAmmoEffectName,  LowAmmoEffectName),
        (AbundentAmmoEffectName,  OneInTheChamberEffectName),
        (AbundentAmmoEffectName,  OneTapEffectName),
        (AbundentAmmoEffectName,  KnifeOnlyEffectName),
        (KnifeOnlyEffectName,     VampireEffectName)
    };

    private static bool CanWorkTogether(IEnumerable<string> selected, string candidate)
    {
        // One Tap is incompatible with every loadout effect.
        if (candidate == OneTapEffectName && selected.Any(s => LoadoutEffects.Contains(s)))
            return false;

        // Symmetric pair check — order of (candidate, existing) does not matter.
        foreach (var existing in selected)
        {
            if (IncompatiblePairs.Contains((candidate, existing)) ||
                IncompatiblePairs.Contains((existing, candidate)))
                return false;
        }

        return true;
    }

    private static void ApplyServerDefaults()
    {
        Server.ExecuteCommand("mp_freezetime 3");
        Server.ExecuteCommand("mp_buytime 0");
        Server.ExecuteCommand("mp_buy_anywhere 0");
        Server.ExecuteCommand("mp_buy_during_immunity 0");
        Server.ExecuteCommand("mp_free_armor 0");
        Server.ExecuteCommand("sv_accelerate_use_weapon_speed 1");
        Server.ExecuteCommand($"sv_accelerate {DefaultAccelerate}");
        Server.ExecuteCommand($"sv_airaccelerate {DefaultAirAccelerate}");
        Server.ExecuteCommand($"ammo_grenade_limit_total {DefaultGrenadeLimit}");
    }

    private static void ApplyRoundEffect(string effectName)
    {
        switch (effectName)
        {
            case LowGravityEffectName:
                ApplyLowGravity();
                break;
            case HighGravityEffectName:
                ApplyHighGravity();
                break;
            case KnifeOnlyEffectName:
                ApplyLoadoutToAllAlivePlayers(KnifeOnlyEffectName);
                break;
            case NoScopeAwpEffectName:
                ApplyLoadoutToAllAlivePlayers(NoScopeAwpEffectName);
                break;
            case PistolsOnlyEffectName:
                ApplyLoadoutToAllAlivePlayers(PistolsOnlyEffectName);
                break;
            case HeadshotsOnlyEffectName:
                _headshotOnlyActive = true;
                break;
            case OneInTheChamberEffectName:
                ApplyLoadoutToAllAlivePlayers(OneInTheChamberEffectName);
                break;
            case InvisibleRadarEffectName:
                Server.ExecuteCommand("sv_disable_radar 1");
                break;
            case RandomWeaponsEffectName:
                ApplyLoadoutToAllAlivePlayers(RandomWeaponsEffectName);
                break;
            case ScoutzKnivezEffectName:
                ApplyLoadoutToAllAlivePlayers(ScoutzKnivezEffectName);
                break;
            case Hp35EffectName:
                SetAllAlivePlayersHp(35);
                break;
            case Hp150EffectName:
                SetAllAlivePlayersHp(150);
                break;
            case NoArmorEffectName:
                break;
            case NadeFrenzyEffectName:
                ApplyNadeFrenzy();
                break;
            case ShotgunsOnlyEffectName:
                ApplyLoadoutToAllAlivePlayers(ShotgunsOnlyEffectName);
                break;
            case SmgRushEffectName:
                ApplyLoadoutToAllAlivePlayers(SmgRushEffectName);
                break;
            case HeavyOnlyEffectName:
                ApplyLoadoutToAllAlivePlayers(HeavyOnlyEffectName);
                break;
            case PistolRouletteEffectName:
                ApplyLoadoutToAllAlivePlayers(PistolRouletteEffectName);
                break;
            case DeagleDuelEffectName:
                ApplyLoadoutToAllAlivePlayers(DeagleDuelEffectName);
                break;
            case AutoSniperEffectName:
                ApplyLoadoutToAllAlivePlayers(AutoSniperEffectName);
                break;
            case LowAmmoEffectName:
                ApplyLowAmmo();
                break;
            case SlowMovementEffectName:
                Server.ExecuteCommand("sv_accelerate_use_weapon_speed 1");
                Server.ExecuteCommand($"sv_maxspeed {SlowMovementSpeed}");
                Server.ExecuteCommand($"sv_accelerate {SlowAccelerate}");
                Server.ExecuteCommand($"sv_airaccelerate {SlowAirAccelerate}");
                ApplyMovementMultiplierToAllAlivePlayers(SlowMovementMultiplier);
                ApplyMaxSpeedToAllAlivePlayers(SlowMovementSpeed);
                break;
            case VampireEffectName:
                _vampireActive = true;
                break;
            case OneTapEffectName:
                ApplyOneTap();
                break;
            case ClumsyEffectName:
                _clumsyActive = true;
                break;
            default:
                ResetGravity();
                break;
        }
    }

    private static void ApplyLowGravity()
    {
        Server.ExecuteCommand($"sv_gravity {LowGravityValue}");
    }

    private static void ApplyHighGravity()
    {
        Server.ExecuteCommand($"sv_gravity {HighGravityValue}");
    }

    private static void ApplyLoadoutToAllAlivePlayers(string loadout)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (!player.IsValid || player.PawnIsAlive != true)
            {
                continue;
            }

            ApplyLoadoutToPlayer(player, loadout);
        }
    }

    private static void ApplyNoScopeAwpToPlayer(CCSPlayerController player)
    {
        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        player.RemoveWeapons();
        player.GiveNamedItem("weapon_knife");
        player.GiveNamedItem("weapon_awp");
    }

    private static string? GetActiveLoadoutEffect()
    {
        return LoadoutEffects.FirstOrDefault(ActiveEffects.Contains);
    }

    private static void ApplyLoadoutToPlayer(CCSPlayerController player, string loadoutEffect)
    {
        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        switch (loadoutEffect)
        {
            case KnifeOnlyEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                break;
            case NoScopeAwpEffectName:
                ApplyNoScopeAwpToPlayer(player);
                break;
            case PistolsOnlyEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem(RandomPistolWeaponPool[Random.Shared.Next(RandomPistolWeaponPool.Length)]);
                break;
            case OneInTheChamberEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem("weapon_deagle");
                ApplyOneInTheChamberAmmoToPlayer(player);
                break;
            case RandomWeaponsEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem(RandomPistolWeaponPool[Random.Shared.Next(RandomPistolWeaponPool.Length)]);
                player.GiveNamedItem(RandomPrimaryWeaponPool[Random.Shared.Next(RandomPrimaryWeaponPool.Length)]);
                break;
            case ScoutzKnivezEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem("weapon_ssg08");
                break;
            case ShotgunsOnlyEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem(ShotgunWeaponPool[Random.Shared.Next(ShotgunWeaponPool.Length)]);
                break;
            case SmgRushEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem(SmgWeaponPool[Random.Shared.Next(SmgWeaponPool.Length)]);
                break;
            case HeavyOnlyEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem(HeavyWeaponPool[Random.Shared.Next(HeavyWeaponPool.Length)]);
                break;
            case PistolRouletteEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem(RandomPistolWeaponPool[Random.Shared.Next(RandomPistolWeaponPool.Length)]);
                break;
            case DeagleDuelEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem("weapon_deagle");
                break;
            case AutoSniperEffectName:
                player.RemoveWeapons();
                player.GiveNamedItem("weapon_knife");
                player.GiveNamedItem(AutoSniperPool[Random.Shared.Next(AutoSniperPool.Length)]);
                break;
        }

        QueueAmmoEnforcementAfterGive(player);
    }

    private static void QueueAmmoEnforcementAfterGive(CCSPlayerController player)
    {
        Server.NextWorldUpdate(() =>
        {
            ApplyLowAmmoToPlayer(player);
            ApplyOneInTheChamberAmmoToPlayer(player);
        });

        Server.NextWorldUpdate(() => Server.NextWorldUpdate(() =>
        {
            ApplyLowAmmoToPlayer(player);
            ApplyOneInTheChamberAmmoToPlayer(player);
        }));
    }

    private static void ApplyOneInTheChamberAmmo()
    {
        if (!ActiveEffects.Contains(OneInTheChamberEffectName))
        {
            return;
        }

        foreach (var player in Utilities.GetPlayers())
        {
            ApplyOneInTheChamberAmmoToPlayer(player);
        }
    }

    private static void EnforceOneInTheChamberAmmoCap()
    {
        if (!ActiveEffects.Contains(OneInTheChamberEffectName))
        {
            return;
        }

        foreach (var player in Utilities.GetPlayers())
        {
            ClampOneInTheChamberClipToMaxOne(player);
        }
    }

    private static void ClampOneInTheChamberClipToMaxOne(CCSPlayerController player)
    {
        if (!ActiveEffects.Contains(OneInTheChamberEffectName))
        {
            return;
        }

        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        var weaponServices = player.PlayerPawn.Value?.WeaponServices;
        if (weaponServices == null)
        {
            return;
        }

        foreach (var weaponHandle in weaponServices.MyWeapons)
        {
            var weapon = weaponHandle.Value;
            if (weapon == null || !weapon.IsValid)
            {
                continue;
            }

            if (!string.Equals(weapon.DesignerName, "weapon_deagle", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (weapon.Clip1 > 1)
            {
                weapon.Clip1 = 1;
                try
                {
                    Utilities.SetStateChanged(weapon.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_iClip1");
                }
                catch
                {
                }
            }

            try
            {
                var reserveAmmo = weapon.ReserveAmmo;
                for (var index = 0; index < reserveAmmo.Length; index++)
                {
                    reserveAmmo[index] = 0;
                }

                Utilities.SetStateChanged(weapon.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_pReserveAmmo");
            }
            catch
            {
            }
        }
    }

    private static void ApplyOneInTheChamberAmmoToPlayer(CCSPlayerController player)
    {
        if (!ActiveEffects.Contains(OneInTheChamberEffectName))
        {
            return;
        }

        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        var weaponServices = player.PlayerPawn.Value?.WeaponServices;
        if (weaponServices == null)
        {
            return;
        }

        foreach (var weaponHandle in weaponServices.MyWeapons)
        {
            var weapon = weaponHandle.Value;
            if (weapon == null || !weapon.IsValid)
            {
                continue;
            }

            if (!string.Equals(weapon.DesignerName, "weapon_deagle", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            weapon.Clip1 = 1;
            try
            {
                Utilities.SetStateChanged(weapon.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_iClip1");
            }
            catch
            {
            }

            try
            {
                var reserveAmmo = weapon.ReserveAmmo;
                for (var index = 0; index < reserveAmmo.Length; index++)
                {
                    reserveAmmo[index] = 0;
                }

                Utilities.SetStateChanged(weapon.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_pReserveAmmo");
            }
            catch
            {
            }
        }
    }

    private static void SetAllAlivePlayersHp(int hp)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            if (!player.IsValid || player.PawnIsAlive != true)
            {
                continue;
            }

            var pawn = player.PlayerPawn.Value;
            if (pawn is null || !pawn.IsValid)
            {
                continue;
            }

            pawn.Health = hp;
        }
    }

    private static int? GetActiveHpValue()
    {
        if (ActiveEffects.Contains(Hp35EffectName))
        {
            return 35;
        }

        if (ActiveEffects.Contains(Hp150EffectName))
        {
            return 150;
        }

        return null;
    }

    private static void ApplyActiveHpEffect()
    {
        var hpValue = GetActiveHpValue();
        if (!hpValue.HasValue)
        {
            return;
        }

        SetAllAlivePlayersHp(hpValue.Value);
    }

    private static void ApplyActiveHpEffectToPlayer(CCSPlayerController player)
    {
        var hpValue = GetActiveHpValue();
        if (!hpValue.HasValue)
        {
            return;
        }

        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        var pawn = player.PlayerPawn.Value;
        if (pawn is null || !pawn.IsValid)
        {
            return;
        }

        pawn.Health = hpValue.Value;
    }

    private static void ApplyArmorToAllPlayers()
    {
        Server.ExecuteCommand("mp_free_armor 0");

        foreach (var player in Utilities.GetPlayers())
        {
            if (!player.IsValid || player.PawnIsAlive != true)
            {
                continue;
            }

            player.GiveNamedItem("item_assaultsuit");
        }
    }

    private static void ApplyNoArmor()
    {
        if (!ActiveEffects.Contains(NoArmorEffectName))
        {
            return;
        }

        Server.ExecuteCommand("mp_free_armor 0");

        foreach (var player in Utilities.GetPlayers())
        {
            ApplyNoArmorToPlayer(player);
        }
    }

    private static void ApplyNoArmorToPlayer(CCSPlayerController player)
    {
        if (!ActiveEffects.Contains(NoArmorEffectName))
        {
            return;
        }

        if (!player.IsValid)
        {
            return;
        }

        player.PawnArmor = 0;
        player.PawnHasHelmet = false;
    }

    private static void ApplyNadeFrenzy()
    {
        if (!ActiveEffects.Contains(NadeFrenzyEffectName))
        {
            return;
        }

        Server.ExecuteCommand($"ammo_grenade_limit_total {NadeFrenzyGrenadeLimit}");

        foreach (var player in Utilities.GetPlayers())
        {
            ApplyNadeFrenzyToPlayer(player);
        }
    }

    private static void ApplyNadeFrenzyToPlayer(CCSPlayerController player)
    {
        if (!ActiveEffects.Contains(NadeFrenzyEffectName))
        {
            return;
        }

        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        foreach (var grenadeName in NadeFrenzyGrenades)
        {
            if (!PlayerHasWeapon(player, grenadeName))
            {
                player.GiveNamedItem(grenadeName);
            }
        }
    }

    private static void ReplenishNadeFrenzyGrenades()
    {
        if (!ActiveEffects.Contains(NadeFrenzyEffectName))
        {
            return;
        }

        foreach (var player in Utilities.GetPlayers())
        {
            ApplyNadeFrenzyToPlayer(player);
        }
    }

    private static bool PlayerHasWeapon(CCSPlayerController player, string weaponName)
    {
        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return false;
        }

        var weaponServices = player.PlayerPawn.Value?.WeaponServices;
        if (weaponServices == null)
        {
            return false;
        }

        foreach (var weaponHandle in weaponServices.MyWeapons)
        {
            var weapon = weaponHandle.Value;
            if (weapon == null || !weapon.IsValid)
            {
                continue;
            }

            if (string.Equals(weapon.DesignerName, weaponName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static void ApplyLowAmmo()
    {
        if (!ActiveEffects.Contains(LowAmmoEffectName))
        {
            return;
        }

        foreach (var player in Utilities.GetPlayers())
        {
            ApplyLowAmmoToPlayer(player);
        }
    }

    private static void ApplyLowAmmoToPlayer(CCSPlayerController player)
    {
        if (!ActiveEffects.Contains(LowAmmoEffectName))
        {
            return;
        }

        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        var weaponServices = player.PlayerPawn.Value?.WeaponServices;
        if (weaponServices == null)
        {
            return;
        }

        foreach (var weaponHandle in weaponServices.MyWeapons)
        {
            var weapon = weaponHandle.Value;
            if (weapon == null || !weapon.IsValid)
            {
                continue;
            }

            try
            {
                var reserveAmmo = weapon.ReserveAmmo;
                for (var index = 0; index < reserveAmmo.Length; index++)
                {
                    reserveAmmo[index] = 0;
                }

                Utilities.SetStateChanged(weapon.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_pReserveAmmo");
            }
            catch
            {
            }

            EnforceWeaponReserveAmmo(weapon, 0);
            TryZeroReserveAmmoDirect(weapon);
            ApplyLowAmmoToReservePools(weapon);
        }

        try
        {
            var servicesAmmo = weaponServices.Ammo;
            for (var index = 0; index < servicesAmmo.Length; index++)
            {
                servicesAmmo[index] = 0;
            }
        }
        catch
        {
        }

        TryZeroReserveAmmoDirect(weaponServices);
        ApplyLowAmmoToReservePools(weaponServices);
    }

    private static void ApplyLowAmmoToReservePools(object ammoOwner)
    {
        TryZeroReserveAmmoDirect(ammoOwner);

        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        foreach (var property in ammoOwner.GetType().GetProperties(flags))
        {
            if (!property.CanRead)
            {
                continue;
            }

            if (!property.Name.Contains("Ammo", StringComparison.OrdinalIgnoreCase) &&
                !property.Name.Contains("Reserve", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            object? value;
            try
            {
                value = property.GetValue(ammoOwner);
            }
            catch
            {
                continue;
            }

            if (value == null)
            {
                continue;
            }

            if (property.CanWrite && IsNumericType(property.PropertyType))
            {
                TrySetNumeric(property, ammoOwner, 0);
            }

            TryZeroIndexedCollection(value);
        }

        foreach (var field in ammoOwner.GetType().GetFields(flags))
        {
            if (!field.Name.Contains("Ammo", StringComparison.OrdinalIgnoreCase) &&
                !field.Name.Contains("Reserve", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            object? value;
            try
            {
                value = field.GetValue(ammoOwner);
            }
            catch
            {
                continue;
            }

            if (value == null)
            {
                continue;
            }

            if (IsNumericType(field.FieldType))
            {
                TrySetNumericField(field, ammoOwner, 0);
            }

            TryZeroIndexedCollection(value);
        }
    }

    private static bool IsNumericType(Type type)
    {
        var code = Type.GetTypeCode(Nullable.GetUnderlyingType(type) ?? type);
        return code is TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16
            or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
    }

    private static void TrySetNumeric(PropertyInfo property, object target, int value)
    {
        try
        {
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var converted = Convert.ChangeType(value, underlyingType);
            property.SetValue(target, converted);
        }
        catch
        {
        }
    }

    private static void TrySetNumericField(FieldInfo field, object target, int value)
    {
        try
        {
            var underlyingType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
            var converted = Convert.ChangeType(value, underlyingType);
            field.SetValue(target, converted);
        }
        catch
        {
        }
    }

    private static void TryZeroIndexedCollection(object collection)
    {
        var type = collection.GetType();
        var indexerInt = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, new[] { typeof(int) }, null);
        var indexerUInt = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, new[] { typeof(uint) }, null);

        if ((indexerInt == null || !indexerInt.CanWrite) && (indexerUInt == null || !indexerUInt.CanWrite))
        {
            return;
        }

        var length = GetCollectionLength(collection);
        if (length <= 0)
        {
            return;
        }

        if (indexerInt != null && indexerInt.CanWrite)
        {
            for (var index = 0; index < length; index++)
            {
                TrySetIndexedValue(indexerInt, collection, index);
            }
        }

        if (indexerUInt != null && indexerUInt.CanWrite)
        {
            for (uint index = 0; index < length; index++)
            {
                TrySetIndexedValue(indexerUInt, collection, index);
            }
        }
    }

    private static int GetCollectionLength(object collection)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        foreach (var lengthName in new[] { "Count", "Length", "Size" })
        {
            var property = collection.GetType().GetProperty(lengthName, flags);
            if (property == null || !property.CanRead)
            {
                continue;
            }

            try
            {
                var value = property.GetValue(collection);
                return Convert.ToInt32(value);
            }
            catch
            {
            }
        }

        return 0;
    }

    private static void TrySetIndexedValue(PropertyInfo indexer, object collection, object index)
    {
        try
        {
            var itemType = Nullable.GetUnderlyingType(indexer.PropertyType) ?? indexer.PropertyType;
            var zero = Convert.ChangeType(0, itemType);
            indexer.SetValue(collection, zero, new[] { index });
        }
        catch
        {
        }
    }

    private static void TryZeroReserveAmmoDirect(object ammoOwner)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var reserveAmmo = ammoOwner.GetType().GetProperty("ReserveAmmo", flags);
        if (reserveAmmo == null || !reserveAmmo.CanRead)
        {
            return;
        }

        object? value;
        try
        {
            value = reserveAmmo.GetValue(ammoOwner);
        }
        catch
        {
            return;
        }

        if (value == null)
        {
            return;
        }

        TryZeroIndexedCollection(value);
    }

    private static void EnforceWeaponReserveAmmo(CBasePlayerWeapon weapon, int ammoValue)
    {
        try
        {
            TrySetNamedNumericMember(weapon, "PrimaryReserveAmmoCount", ammoValue);
            TrySetNamedNumericMember(weapon, "m_iPrimaryReserveAmmoCount", ammoValue);

            var reserveAmmo = weapon.GetType().GetProperty("ReserveAmmo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var reserveAmmoValue = reserveAmmo?.GetValue(weapon);
            if (reserveAmmoValue != null)
            {
                var primaryAmmoType = TryGetIntProperty(weapon, "PrimaryAmmoType");
                var secondaryAmmoType = TryGetIntProperty(weapon, "SecondaryAmmoType");

                if (primaryAmmoType.HasValue && primaryAmmoType.Value >= 0)
                {
                    TrySetReserveAmmoByIndex(reserveAmmoValue, primaryAmmoType.Value, ammoValue);
                }

                if (secondaryAmmoType.HasValue && secondaryAmmoType.Value >= 0)
                {
                    TrySetReserveAmmoByIndex(reserveAmmoValue, secondaryAmmoType.Value, ammoValue);
                }

                // Common slots + fallback sweep for weapons where ammo index mapping differs.
                for (var index = 0; index < 8; index++)
                {
                    TrySetReserveAmmoByIndex(reserveAmmoValue, index, ammoValue);
                }
            }

            var vData = weapon.GetType().GetProperty("VData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(weapon);
            if (vData != null)
            {
                var maxReserve = vData.GetType().GetProperty("PrimaryReserveAmmoMax", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (maxReserve != null && maxReserve.CanWrite)
                {
                    var maxType = Nullable.GetUnderlyingType(maxReserve.PropertyType) ?? maxReserve.PropertyType;
                    maxReserve.SetValue(vData, Convert.ChangeType(ammoValue, maxType));
                }
            }

            try
            {
                Utilities.SetStateChanged(weapon.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_pReserveAmmo");
            }
            catch
            {
            }
        }
        catch
        {
        }
    }

    private static void RewardLowAmmoReserveOnKill(CCSPlayerController player)
    {
        if (!ActiveEffects.Contains(LowAmmoEffectName))
        {
            return;
        }

        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        var weaponServices = player.PlayerPawn.Value?.WeaponServices;
        if (weaponServices == null)
        {
            return;
        }

        CBasePlayerWeapon? weapon = null;
        foreach (var weaponHandle in weaponServices.MyWeapons)
        {
            var candidate = weaponHandle.Value;
            if (candidate == null || !candidate.IsValid || candidate.Clip1 < 0)
            {
                continue;
            }

            var name = candidate.DesignerName ?? string.Empty;
            if (name.StartsWith("weapon_knife", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("grenade", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("molotov", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("decoy", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("flashbang", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("smoke", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            weapon = candidate;
            break;
        }

        if (weapon == null || !weapon.IsValid)
        {
            return;
        }

        const int rewardAmount = 1;

        try
        {
            var reserveAmmo = weapon.ReserveAmmo;
            var primaryAmmoType = TryGetIntProperty(weapon, "PrimaryAmmoType");
            var secondaryAmmoType = TryGetIntProperty(weapon, "SecondaryAmmoType");

            var currentPrimaryReserve = TryGetIntProperty(weapon, "PrimaryReserveAmmoCount")
                                     ?? TryGetIntProperty(weapon, "m_iPrimaryReserveAmmoCount")
                                     ?? 0;
            if (currentPrimaryReserve > 0)
            {
                return;
            }

            var hasValidPrimaryIndex = primaryAmmoType.HasValue && primaryAmmoType.Value >= 0 && primaryAmmoType.Value < reserveAmmo.Length;
            var hasValidSecondaryIndex = secondaryAmmoType.HasValue && secondaryAmmoType.Value >= 0 && secondaryAmmoType.Value < reserveAmmo.Length;

            if ((hasValidPrimaryIndex && reserveAmmo[primaryAmmoType!.Value] > 0) ||
                (hasValidSecondaryIndex && reserveAmmo[secondaryAmmoType!.Value] > 0) ||
                (!hasValidPrimaryIndex && !hasValidSecondaryIndex && reserveAmmo.Length > 0 && reserveAmmo[0] > 0))
            {
                return;
            }

            TrySetNamedNumericMember(weapon, "PrimaryReserveAmmoCount", rewardAmount);
            TrySetNamedNumericMember(weapon, "m_iPrimaryReserveAmmoCount", rewardAmount);

            if (primaryAmmoType.HasValue && primaryAmmoType.Value >= 0 && primaryAmmoType.Value < reserveAmmo.Length)
            {
                reserveAmmo[primaryAmmoType.Value] = rewardAmount;
            }

            if (secondaryAmmoType.HasValue && secondaryAmmoType.Value >= 0 && secondaryAmmoType.Value < reserveAmmo.Length)
            {
                reserveAmmo[secondaryAmmoType.Value] = rewardAmount;
            }

            if ((!primaryAmmoType.HasValue || primaryAmmoType.Value < 0 || primaryAmmoType.Value >= reserveAmmo.Length) &&
                (!secondaryAmmoType.HasValue || secondaryAmmoType.Value < 0 || secondaryAmmoType.Value >= reserveAmmo.Length) &&
                reserveAmmo.Length > 0)
            {
                reserveAmmo[0] = rewardAmount;
            }

            Utilities.SetStateChanged(weapon.As<CCSWeaponBase>(), "CBasePlayerWeapon", "m_pReserveAmmo");
        }
        catch
        {
        }
    }

    private static int? TryGetIntProperty(object owner, string propertyName)
    {
        try
        {
            var property = owner.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property == null || !property.CanRead)
            {
                return null;
            }

            var value = property.GetValue(owner);
            if (value == null)
            {
                return null;
            }

            return Convert.ToInt32(value);
        }
        catch
        {
            return null;
        }
    }

    private static void TrySetReserveAmmoByIndex(object reserveAmmo, int index, int ammoValue)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var indexerInt = reserveAmmo.GetType().GetProperty("Item", flags, null, null, new[] { typeof(int) }, null);
        if (indexerInt != null && indexerInt.CanWrite)
        {
            try
            {
                var itemType = Nullable.GetUnderlyingType(indexerInt.PropertyType) ?? indexerInt.PropertyType;
                var converted = Convert.ChangeType(ammoValue, itemType);
                indexerInt.SetValue(reserveAmmo, converted, new object[] { index });
            }
            catch
            {
            }
        }

        var indexerUInt = reserveAmmo.GetType().GetProperty("Item", flags, null, null, new[] { typeof(uint) }, null);
        if (indexerUInt != null && indexerUInt.CanWrite)
        {
            try
            {
                var itemType = Nullable.GetUnderlyingType(indexerUInt.PropertyType) ?? indexerUInt.PropertyType;
                var converted = Convert.ChangeType(ammoValue, itemType);
                indexerUInt.SetValue(reserveAmmo, converted, new object[] { (uint)index });
            }
            catch
            {
            }
        }
    }

    private static void TrySetNamedNumericMember(object owner, string memberName, int value)
    {
        try
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var property = owner.GetType().GetProperty(memberName, flags);
            if (property != null && property.CanWrite)
            {
                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                property.SetValue(owner, Convert.ChangeType(value, targetType));
                return;
            }

            var field = owner.GetType().GetField(memberName, flags);
            if (field != null)
            {
                var targetType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                field.SetValue(owner, Convert.ChangeType(value, targetType));
            }
        }
        catch
        {
        }
    }

    private static void TrySetNamedBooleanMember(object owner, string memberName, bool value)
    {
        try
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var property = owner.GetType().GetProperty(memberName, flags);
            if (property != null && property.CanWrite)
            {
                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (targetType == typeof(bool))
                {
                    property.SetValue(owner, value);
                    return;
                }
            }

            var field = owner.GetType().GetField(memberName, flags);
            if (field != null)
            {
                var targetType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                if (targetType == typeof(bool))
                {
                    field.SetValue(owner, value);
                }
            }
        }
        catch
        {
        }
    }

    private static void TrySetNamedFloatMember(object owner, string memberName, float value)
    {
        try
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var property = owner.GetType().GetProperty(memberName, flags);
            if (property != null && property.CanWrite)
            {
                var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                property.SetValue(owner, Convert.ChangeType(value, targetType));
                return;
            }

            var field = owner.GetType().GetField(memberName, flags);
            if (field != null)
            {
                var targetType = Nullable.GetUnderlyingType(field.FieldType) ?? field.FieldType;
                field.SetValue(owner, Convert.ChangeType(value, targetType));
            }
        }
        catch
        {
        }
    }

    private static void ApplyOneTap()
    {
        _oneTapActive = true;

        foreach (var player in Utilities.GetPlayers())
        {
            if (!player.IsValid || player.PawnIsAlive != true)
            {
                continue;
            }

            ApplyOneTapToPlayer(player);
        }
    }

    private static void ApplyOneTapToPlayer(CCSPlayerController player)
    {
        if (!ActiveEffects.Contains(OneTapEffectName))
        {
            return;
        }

        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        player.RemoveWeapons();
        player.GiveNamedItem("weapon_knife");
        // Each player gets AWP or Scout - one shot, one kill
        var sniper = Random.Shared.Next(2) == 0 ? "weapon_awp" : "weapon_ssg08";
        player.GiveNamedItem(sniper);
        QueueAmmoEnforcementAfterGive(player);
    }

    private static void ClearCurrentEffectState()
    {
        ResetGravity();
        Server.ExecuteCommand("sv_accelerate_use_weapon_speed 1");
        Server.ExecuteCommand($"sv_maxspeed {DefaultMaxSpeed}");
        Server.ExecuteCommand($"sv_accelerate {DefaultAccelerate}");
        Server.ExecuteCommand($"sv_airaccelerate {DefaultAirAccelerate}");
        Server.ExecuteCommand($"ammo_grenade_limit_total {DefaultGrenadeLimit}");
        SetAllAlivePlayersHp(DefaultRoundHp);
        ApplyArmorToAllPlayers();
        Server.ExecuteCommand("sv_disable_radar 0");
        _headshotOnlyActive = false;
        _vampireActive = false;
        _oneTapActive = false;
        _clumsyActive = false;
        ResetAllPlayersMovementMultipliers();
        ResetAllPlayersMaxSpeed();
        ActiveEffects.Clear();
    }

    private static void ApplyActiveMovementEffectToPlayer(CCSPlayerController player)
    {
        if (ActiveEffects.Contains(SlowMovementEffectName))
        {
            SetPlayerMovementMultiplier(player, SlowMovementMultiplier);
            SetPlayerMaxSpeed(player, SlowMovementSpeed);
            return;
        }

        SetPlayerMovementMultiplier(player, DefaultMovementMultiplier);
        SetPlayerMaxSpeed(player, DefaultMaxSpeed);
    }

    private static void ApplyMovementMultiplierToAllAlivePlayers(float multiplier)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            SetPlayerMovementMultiplier(player, multiplier);
        }
    }

    private static void ApplyMaxSpeedToAllAlivePlayers(float maxSpeed)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            SetPlayerMaxSpeed(player, maxSpeed);
        }
    }

    private static void ResetAllPlayersMovementMultipliers()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            SetPlayerMovementMultiplier(player, DefaultMovementMultiplier);
        }
    }

    private static void ResetAllPlayersMaxSpeed()
    {
        foreach (var player in Utilities.GetPlayers())
        {
            SetPlayerMaxSpeed(player, DefaultMaxSpeed);
        }
    }

    private static void SetPlayerMovementMultiplier(CCSPlayerController player, float multiplier)
    {
        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        var pawn = player.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid)
        {
            return;
        }

        var movementServices = TryGetNamedMemberValue(pawn, "MovementServices")
            ?? TryGetNamedMemberValue(pawn, "m_pMovementServices")
            ?? TryGetNamedMemberValue(pawn, "CPlayer_MovementServices")
            ?? TryGetNamedMemberValue(pawn, "CCSPlayer_MovementServices");

        var effectiveMultiplier = Math.Clamp(multiplier, 0.1f, 3.0f);

        TrySetNamedFloatMember(pawn, "LaggedMovementValue", multiplier);
        TrySetNamedFloatMember(pawn, "m_flLaggedMovementValue", multiplier);

        if (movementServices != null)
        {
            TrySetNamedFloatMember(movementServices, "LaggedMovementValue", effectiveMultiplier);
            TrySetNamedFloatMember(movementServices, "m_flLaggedMovementValue", effectiveMultiplier);
        }

        TrySetNamedFloatMember(pawn, "LaggedMovementValue", effectiveMultiplier);
        TrySetNamedFloatMember(pawn, "m_flLaggedMovementValue", effectiveMultiplier);
        TrySetNamedFloatMember(player, "LaggedMovementValue", effectiveMultiplier);
        TrySetNamedFloatMember(player, "m_flLaggedMovementValue", effectiveMultiplier);

        TrySetNamedFloatMember(pawn, "VelocityModifier", effectiveMultiplier);
        TrySetNamedFloatMember(pawn, "m_flVelocityModifier", effectiveMultiplier);
        TrySetNamedFloatMember(player, "VelocityModifier", effectiveMultiplier);
        TrySetNamedFloatMember(player, "m_flVelocityModifier", effectiveMultiplier);

        if (movementServices != null)
        {
            TrySetNamedFloatMember(movementServices, "VelocityModifier", effectiveMultiplier);
            TrySetNamedFloatMember(movementServices, "m_flVelocityModifier", effectiveMultiplier);
        }

    }

    private static void SetPlayerMaxSpeed(CCSPlayerController player, float maxSpeed)
    {
        if (!player.IsValid || player.PawnIsAlive != true)
        {
            return;
        }

        var pawn = player.PlayerPawn.Value;
        if (pawn == null || !pawn.IsValid)
        {
            return;
        }

        TrySetNamedFloatMember(player, "MaxSpeed", maxSpeed);
        TrySetNamedFloatMember(player, "Maxspeed", maxSpeed);
        TrySetNamedFloatMember(player, "m_flMaxspeed", maxSpeed);
        TrySetNamedFloatMember(pawn, "MaxSpeed", maxSpeed);
        TrySetNamedFloatMember(pawn, "Maxspeed", maxSpeed);
        TrySetNamedFloatMember(pawn, "m_flMaxspeed", maxSpeed);

        var movementServices = TryGetNamedMemberValue(pawn, "MovementServices")
            ?? TryGetNamedMemberValue(pawn, "m_pMovementServices")
            ?? TryGetNamedMemberValue(pawn, "CPlayer_MovementServices")
            ?? TryGetNamedMemberValue(pawn, "CCSPlayer_MovementServices");

        if (movementServices != null)
        {
            TrySetNamedFloatMember(movementServices, "MaxSpeed", maxSpeed);
            TrySetNamedFloatMember(movementServices, "Maxspeed", maxSpeed);
            TrySetNamedFloatMember(movementServices, "m_flMaxspeed", maxSpeed);
        }
    }


    private static object? TryGetNamedMemberValue(object owner, string memberName)
    {
        try
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var property = owner.GetType().GetProperty(memberName, flags);
            if (property != null && property.CanRead)
            {
                return property.GetValue(owner);
            }

            var field = owner.GetType().GetField(memberName, flags);
            return field?.GetValue(owner);
        }
        catch
        {
            return null;
        }
    }

    private void ShowEffectRulesCommand(CCSPlayerController? player, CommandInfo command)
    {
        command.ReplyToCommand("[Round Effect Rules] Loadouts (pick 1): Knife Only, AWP, Pistols Only, One in the Chamber, Random Weapons, ScoutzKnivez, Shotguns Only, SMG Rush, Heavy Only, Pistol Roulette, Deagle Duel, Auto-Sniper Mayhem");
        command.ReplyToCommand("[Round Effect Rules] Modifiers (pick 2): Low Gravity, High Gravity, Headshots Only, Invisible Radar, 35 HP, 150 HP, No Armor, Nade Frenzy, Low Ammo, Slow Movement, Vampire, One Tap, Clumsy");
        command.ReplyToCommand("[Round Effect Rules] Can't combine: Knife Only + Headshots Only");
        command.ReplyToCommand("[Round Effect Rules] Can't combine: 35 HP + 150 HP");
        command.ReplyToCommand("[Round Effect Rules] Can't combine: Low Gravity + High Gravity");
        command.ReplyToCommand("[Round Effect Rules] Can't combine: Knife Only + Low Ammo");
        command.ReplyToCommand("[Round Effect Rules] Can't combine: One Tap + Headshots Only");
        command.ReplyToCommand("[Round Effect Rules] Can't combine: One Tap + any other loadout (Knife/AWP/Pistols/Chamber/Random/ScoutzKnivez/Shotguns/SMG/Heavy/Roulette/Deagle/Auto-Sniper)");
    }

    private void ClearEffectCommand(CCSPlayerController? player, CommandInfo command)
    {
        ClearCurrentEffectState();
        command.ReplyToCommand("[Round Effect] Cleared active forced effect state.");
    }

    private void ForceEffects(CommandInfo command, params string[] effectNames)
    {
        ClearCurrentEffectState();

        foreach (var effectName in effectNames)
        {
            ActiveEffects.Add(effectName);
            ApplyRoundEffect(effectName);
        }

        if (effectNames.Contains(NoArmorEffectName))
        {
            ApplyNoArmor();
        }
        else
        {
            ApplyArmorToAllPlayers();
        }

        command.ReplyToCommand($"[Round Effect] Forced effects: {string.Join(", ", effectNames)}");
        Server.PrintToChatAll($" \x04[Round Effects]\x01 Admin forced: \x05{string.Join("\x01 | \x05", effectNames)}");
        foreach (var p in Utilities.GetPlayers())
        {
            p.PrintToCenter($"{string.Join(" | ", effectNames)}");
        }
    }

    private static void ResetGravity()
    {
        Server.ExecuteCommand($"sv_gravity {DefaultGravity}");
    }
}
