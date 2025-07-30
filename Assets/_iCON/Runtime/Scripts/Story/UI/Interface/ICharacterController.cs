using CryStar.Story.Enums;
using CryStar.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace CryStar.CryStar.UI
{
    /// <summary>
    /// キャラクター画像制御
    /// </summary>
    public interface ICharacterController : IResettable
    {
        void SetupCharacter(float scale, Vector3 position);
        UniTask<Tween> CharacterEntry(CharacterPositionType position, string fileName, float duration);
        UniTask<Tween> ChangeCharacter(CharacterPositionType position, string fileName, float duration);
        Tween CharacterExit(CharacterPositionType position, float duration);
        void HideAllCharacters();
        bool IsCharacterVisible(CharacterPositionType position);
    }
}