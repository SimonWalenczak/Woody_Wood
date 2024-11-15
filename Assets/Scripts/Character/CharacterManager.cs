using Common;
using UnityEngine;

namespace Character
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        #region Properties

        [field: SerializeField] public CharacterController CharacterController { get; private set; }
        [field: SerializeField] public CharacterAnimation CharacterAnimation { get; private set; }

        #endregion

        #region Methods

        #endregion
    }
}