namespace FrameTrapped.StreetFighterLibrary.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    public class HitViewModel
    {
        public enum BlockTypeEnum
        {
            High,
            Mid,
            Low,
            Techable,
            Unblockable
        }

        public enum CancelAbilityEnum
        {
            Chain,
            Dash,
            Special,
            Super,
            Ultra
        }

        private BlockTypeEnum _blockType;
        private int _damage;
        private int _lateDamage;
        private int _stun;
        private int _lateStun;
        private int _meterGain;
        private List<CancelAbilityEnum> _cancelAbility;
        private int _startup;
        private int _active;
        private int _recovery;
        private int _onBlockAdvantage;
        private int _onHitAdvantage;
        private string _notes;

        public BlockTypeEnum BlockType { get { return _blockType; } }
        public int Damage { get { return _damage; } }
        public int LateDamage { get { return _lateDamage; } }
        public int Stun { get { return _stun; } }
        public int LateStun { get { return _lateStun; } }
        public int MeterGain { get { return _meterGain; } }
        public List<CancelAbilityEnum> CancelAbility { get { return _cancelAbility; } }
        public string CancelString{ get { return string.Join("/", _cancelAbility.Distinct().Select(o => o.ToString().Substring(0, 2))); } }
        public int Startup { get { return _startup; } }
        public int Active { get { return _active; } }
        public int Recovery { get { return _recovery; } }
        public int OnBlockAdvantage { get { return _onBlockAdvantage; } }
        public int OnHitAdvantage { get { return _onHitAdvantage; } }
        public string Notes { get { return _notes; } }

        public HitViewModel(BlockTypeEnum blockType,
            int damage,
            int lateDamage,
            int stun,
            int lateStun,
            int meterGain,
            CancelAbilityEnum[] cancelAbility,
            int startup,
            int active,
            int recovery,
            int onBlockAdvantage,
            int onHitAdvantage,
            string notes)
        {
            _blockType = blockType;
            _damage = damage;
            _lateDamage = lateDamage;
            _stun = stun;
            _lateStun = lateStun;
            _meterGain = meterGain;
            _cancelAbility = new List<CancelAbilityEnum>();
            _cancelAbility.AddRange(cancelAbility);
            _startup = startup;
            _active = active;
            _recovery = recovery;
            _onBlockAdvantage = onBlockAdvantage;
            _onHitAdvantage = onHitAdvantage;
            _notes = notes;
        }
    }
}
