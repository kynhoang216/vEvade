namespace vEvade.SpecialSpells
{
    #region
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using vEvade.Helpers;
    using vEvade.Spells;

    using SpellData = vEvade.Spells.SpellData;

    #endregion

    public class Zac : IChampionManager
    {
        #region Static Fields

        private static int lastETick;

        #endregion

        #region Public Methods and Operators

        public void LoadSpecialSpell(SpellData spellData)
        {
            if (spellData.MenuName != "ZacE")
            {
                return;
            }

            Dash.OnDash += (sender, args) => OnDash(sender, args, spellData);
            Obj_AI_Base.OnPlayAnimation += (sender, args) => OnPlayAnimation(sender, args, spellData);
        }

        #endregion

        #region Methods

        private static void OnDash(Obj_AI_Base sender, Dash.DashEventArgs args, SpellData data)
        {
            var caster = sender as AIHeroClient;

            if (caster == null || !caster.IsValid || (!caster.IsEnemy && !Configs.Debug)
                || caster.ChampionName != data.ChampName)
            {
                return;
            }

            if (Utils.GameTimeTickCount - lastETick <= 100)
            {
                SpellDetector.AddSpell(caster, args.StartPos, args.EndPos, data, null, SpellType.None, true, lastETick);
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args, SpellData data)
        {
            var caster = sender as AIHeroClient;

            if (caster == null || !caster.IsValid || (!caster.IsEnemy && !Configs.Debug)
                || caster.ChampionName != data.ChampName)
            {
                return;
            }

            if (args.Animation == "af176358")
            {
                lastETick = Utils.GameTimeTickCount;
            }
        }

        #endregion
    }
}