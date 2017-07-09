namespace vEvade.EvadeSpells
{
    #region
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;

    using vEvade.Helpers;

    #endregion

    public enum SpellValidTargets
    {
        AllyMinions,

        EnemyMinions,

        AllyWards,

        EnemyWards,

        AllyChampions,

        EnemyChampions
    }

    public class EvadeSpellData
    {
        #region Fields

        public bool IsSummonerSpell;

        public bool CanShieldAllies;

        public string CheckSpellName = "";

        public int Delay;

        public bool FixedRange;

        public bool Invert;

        public bool IsBlink;

        public bool IsDash;

        public bool IsInvulnerability;

        public bool IsItem;

        public bool IsMovementSpeedBuff;

        public bool IsShield;

        public bool IsSpellShield;

        public float MaxRange;

        public string MenuName;

        public MoveSpeedAmount MoveSpeedTotalAmount;

        public bool RequiresPreMove;

        public bool SelfCast;

        public SpellSlot Slot;

        public int Speed;

        public SpellValidTargets[] ValidTargets = { };

        private int dangerLevel;

        #endregion

        #region Constructors and Destructors

        public EvadeSpellData()
        {
        }

        public EvadeSpellData(string menuName, int dangerLevel)
        {
            this.MenuName = menuName;
            this.DangerLevel = dangerLevel;
        }

        #endregion

        #region Delegates

        public delegate float MoveSpeedAmount();

        #endregion

        #region Public Properties

        public int DangerLevel
        {
            get
            {
                return Configs.Menu["ES_" + this.MenuName + "_DangerLvl"] != null
                           ? Configs.Menu["ES_" + this.MenuName + "_DangerLvl"].Cast<Slider>().CurrentValue
                           : this.dangerLevel;
            }
            set
            {
                this.dangerLevel = value;
            }
        }

        public bool Enabled
            =>
                Configs.Menu["ES_" + this.MenuName + "_Enabled"] == null
                || Configs.Menu["ES_" + this.MenuName + "_Enabled"].Cast<CheckBox>().CurrentValue;

        public bool IsReady
            =>
                !this.IsItem
                && (string.IsNullOrEmpty(this.CheckSpellName)
                    || ObjectManager.Player.Spellbook.GetSpell(this.Slot).Name == this.CheckSpellName) && //&& this.Slot.IsReady{};
            ((IsSummonerSpell && ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready) ||
                     (!IsSummonerSpell && ObjectManager.Player.Spellbook.CanUseSpell(Slot) == SpellState.Ready));

        public bool IsTargetted => this.ValidTargets != null && this.ValidTargets.Length > 0;

        #endregion
    }

    public class DashData : EvadeSpellData
    {
        #region Constructors and Destructors

        public DashData(
            string menuName,
            SpellSlot slot,
            float range,
            bool fixedRange,
            int delay,
            int speed,
            int dangerLevel)
        {
            this.MenuName = menuName;
            this.Slot = slot;
            this.MaxRange = range;
            this.FixedRange = fixedRange;
            this.Delay = delay;
            this.Speed = speed;
            this.DangerLevel = dangerLevel;
            this.IsDash = true;
        }

        #endregion
    }

    public class BlinkData : EvadeSpellData
    {
        #region Constructors and Destructors

        public BlinkData(string menuName, SpellSlot slot, float range, int delay, int dangerLevel, bool isSummonerSpell = false)
        {
            this.MenuName = menuName;
            this.Slot = slot;
            this.MaxRange = range;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsBlink = true;
            IsSummonerSpell = isSummonerSpell;

        }

        #endregion
    }

    public class InvulnerabilityData : EvadeSpellData
    {
        #region Constructors and Destructors

        public InvulnerabilityData(string menuName, SpellSlot slot, int delay, int dangerLevel)
        {
            this.MenuName = menuName;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsInvulnerability = true;
        }

        #endregion
    }

    public class ShieldData : EvadeSpellData
    {
        #region Constructors and Destructors

        public ShieldData(string menuName, SpellSlot slot, int delay, int dangerLevel, bool isSpellShield = false)
        {
            this.MenuName = menuName;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.IsSpellShield = isSpellShield;
            this.IsShield = !this.IsSpellShield;
        }

        #endregion
    }

    public class MoveBuffData : EvadeSpellData
    {
        #region Constructors and Destructors

        public MoveBuffData(string menuName, SpellSlot slot, int delay, int dangerLevel, MoveSpeedAmount amount)
        {
            this.MenuName = menuName;
            this.Slot = slot;
            this.Delay = delay;
            this.DangerLevel = dangerLevel;
            this.MoveSpeedTotalAmount = amount;
            this.IsMovementSpeedBuff = true;
        }

        #endregion
    }
}