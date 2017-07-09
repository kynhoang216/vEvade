namespace vEvade.Helpers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using EloBuddy;
    using EloBuddy.SDK.Menu;
    using LeagueSharp.Common;
    using EloBuddy.SDK.Menu.Values;

    using vEvade.Core;
    using vEvade.EvadeSpells;
    using vEvade.SpecialSpells;
    using vEvade.Spells;

    using SpellData = vEvade.Spells.SpellData;
    using SharpDX;
    using EloBuddy.SDK;

    #endregion

    internal static class Configs
    {
        #region Constants

        public const int CrossingTime = 250;

        public const int EvadePointChangeTime = 300;

        public const int EvadingFirstTime = 250;

        public const int EvadingRouteChangeTime = 250;

        public const int EvadingSecondTime = 80;

        public const int ExtraSpellRadius = 9;

        public const int ExtraSpellRange = 20;

        public const int GridSize = 10;

        public const int PathFindingInnerDistance = 35;

        public const int PathFindingOuterDistance = 60;

        #endregion

        #region Static Fields

        public static bool Debug;

        public static Menu Menu, evadeSpells, skillShots, shielding, collision, drawings, misc;
        public static Color EnabledColor, DisabledColor, MissileColor;

        private static readonly Dictionary<string, IChampionManager> ChampionManagers =
            new Dictionary<string, IChampionManager>();

        #endregion

        #region Public Properties

        public static int CheckBlock => misc["CheckBlock"].Cast<Slider>().CurrentValue;

        public static bool CheckCollision => misc["CheckCollision"].Cast<CheckBox>().CurrentValue;

        public static bool CheckHp => misc["CheckHp"].Cast<CheckBox>().CurrentValue;

        public static bool DodgeCircle => misc["DodgeCircle"].Cast<CheckBox>().CurrentValue;

        public static bool DodgeCone => misc["DodgeCone"].Cast<CheckBox>().CurrentValue;

        public static bool DodgeDangerous => Menu["DodgeDangerous"].Cast<KeyBind>().CurrentValue;

        public static int DodgeFoW => misc["DodgeFoW"].Cast<Slider>().CurrentValue;

        public static bool DodgeLine => misc["DodgeLine"].Cast<CheckBox>().CurrentValue;

        public static bool DodgeTrap => misc["DodgeTrap"].Cast<CheckBox>().CurrentValue;

        public static bool DrawSpells => drawings["DrawSpells"].Cast<CheckBox>().CurrentValue;

        public static bool DrawStatus => drawings["DrawStatus"].Cast<CheckBox>().CurrentValue;

        public static bool Enabled => Menu["Enabled"].Cast<KeyBind>().CurrentValue;

        #endregion

        #region Public Methods and Operators

        public static void CreateMenu()
        {
            Menu = MainMenu.AddMenu("vEvade", "vEvade");

             skillShots = Menu.AddSubMenu("Spells", "Spells");

            foreach (var hero in EntityManager.Heroes.AllHeroes.Where(i => i.IsEnemy || Debug))
            {
                foreach (var spell in
                    SpellDatabase.Spells.Where(
                        i =>
                        !Evade.OnProcessSpells.ContainsKey(i.SpellName)
                        && (i.IsSummoner || i.ChampName == hero.ChampionName)))
                {
                    if (spell.IsSummoner && hero.GetSpellSlotFromName(spell.SpellName) != SpellSlot.Summoner1
                        && hero.GetSpellSlotFromName(spell.SpellName) != SpellSlot.Summoner2)
                    {
                        continue;
                    }

                    Evade.OnProcessSpells.Add(spell.SpellName, spell);

                    foreach (var name in spell.ExtraSpellNames)
                    {
                        Evade.OnProcessSpells.Add(name, spell);
                    }

                    if (!string.IsNullOrEmpty(spell.MissileName))
                    {
                        Evade.OnMissileSpells.Add(spell.MissileName, spell);
                    }

                    foreach (var name in spell.ExtraMissileNames)
                    {
                        Evade.OnMissileSpells.Add(name, spell);
                    }

                    if (!string.IsNullOrEmpty(spell.TrapName))
                    {
                        Evade.OnTrapSpells.Add(spell.TrapName, spell);
                    }

                    //LoadSpecialSpell(spell);

                    var txt = "S_" + spell.MenuName;
                    skillShots.AddGroupLabel(txt);


                    skillShots.Add("_DangerLvl" + txt, new Slider("Danger Level", spell.DangerValue, 1, 5));
                    skillShots.Add("_IsDangerous" + txt, new CheckBox("Is Dangerous", spell.IsDangerous));
                    skillShots.Add(txt + "_IgnoreHp", new Slider("Ignore If Hp >", !spell.IsDangerous ? 65 : 100, 1));
                    skillShots.Add(txt + "_Draw", new CheckBox("Draw", true));
                    skillShots.Add(txt + "_Enabled", new CheckBox("Enabled", !spell.DisabledByDefault));
                }
            }

            //Menu.AddSubMenu(spells);

            var evadeSpells = Menu.AddSubMenu("Evade Spells", "EvadeSpells");

            foreach (var spell in EvadeSpellDatabase.Spells)
            {
                var txt = "ES_" + spell.MenuName;
                evadeSpells.AddGroupLabel(txt);
                evadeSpells.Add("_DangerLvl" + txt, new Slider("Danger Level", spell.DangerLevel, 1, 5));

                if (spell.IsTargetted && spell.ValidTargets.Contains(SpellValidTargets.AllyWards))
                {
                    evadeSpells.Add("_WardJump" + txt, new CheckBox("Ward Jump", false));
                }

                evadeSpells.Add("_Enabled" + txt, new CheckBox("Enabled"));
                //evadeSpells.Add(subMenu);
            }


            var shielding = Menu.AddSubMenu("Shield Ally", "ShieldAlly");

            foreach (var ally in EntityManager.Heroes.Allies.Where(i => !i.IsMe))
            {
                shielding.Add("SA_" + ally.ChampionName, new CheckBox(ally.ChampionName, true));
            }



            var misc = Menu.AddSubMenu("Misc", "Misc");
            misc.Add("CheckCollision", new CheckBox("Check Collision", false));
            misc.Add("CheckHp", new CheckBox("Check Player Hp", false));
            misc.AddStringList("CheckBlock", "Block Cast While Dodge", new[] { "No", "Only Dangerous", "Always" }, 1);
            misc.AddStringList("DodgeFoW", "Dodge FoW Spells", new[] { "Off", "Track", "Dodge" }, 2);
            misc.Add("DodgeLine", new CheckBox("Dodge Line Spells"));
            misc.Add("DodgeCircle", new CheckBox("Dodge Circle Spells"));
            misc.Add("DodgeCone", new CheckBox("Dodge Cone Spells"));
            misc.Add("DodgeTrap", new CheckBox("Dodge Traps"));

            var draw = Menu.AddSubMenu("Draw", "Draw");
            draw.Add("DrawSpells", new CheckBox("Draw Spells", true));
            draw.Add("DrawStatus", new CheckBox("Draw Status", true));

            Menu.Add("Enabled", new KeyBind("Enabled", true, KeyBind.BindTypes.PressToggle));
            Menu.Add("DodgeDangerous", new KeyBind("Dodge Only Dangerous", false, KeyBind.BindTypes.HoldActive));
        }

        #endregion

        #region Methods

        private static void LoadSpecialSpell(SpellData spell)
        {
            if (ChampionManagers.ContainsKey(spell.ChampName))
            {
                ChampionManagers[spell.ChampName].LoadSpecialSpell(spell);
            }

            ChampionManagers["AllChampions"].LoadSpecialSpell(spell);
        }

        private static void LoadSpecialSpellPlugins()
        {
            ChampionManagers.Add("AllChampions", new AllChampions());

            foreach (var hero in EntityManager.Heroes.AllHeroes.Where(i => i.IsEnemy || Debug))
            {
                var plugin =
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .FirstOrDefault(
                            i => i.IsClass && i.Namespace == "vEvade.SpecialSpells" && i.Name == hero.ChampionName);

                if (plugin != null && !ChampionManagers.ContainsKey(hero.ChampionName))
                {
                    ChampionManagers.Add(hero.ChampionName, (IChampionManager)NewInstance(plugin));
                }
            }
        }

        private static object NewInstance(Type type)
        {
            var target = type.GetConstructor(Type.EmptyTypes);
            var dynamic = new DynamicMethod(string.Empty, type, new Type[0], target.DeclaringType);
            var il = dynamic.GetILGenerator();
            il.DeclareLocal(target.DeclaringType);
            il.Emit(OpCodes.Newobj, target);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            var method = (Func<object>)dynamic.CreateDelegate(typeof(Func<object>));

            return method();
        }

        #endregion
    }
}