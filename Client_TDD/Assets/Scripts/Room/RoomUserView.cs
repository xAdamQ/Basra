using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// it's dependent on player
/// </summary>
public class RoomUserView : MinUserView
{
    [SerializeField] private Image turnFillImage;

    private static Color
     TurnFillStartColor = new Color(1, .815f, 0),
     TurnFillEndColor = new Color(1, 0, 0);

    public void SetTurnFill(float progress)
    {
        turnFillImage.fillAmount = 1f - progress;
        turnFillImage.color = Color.Lerp(TurnFillStartColor, TurnFillEndColor, progress);
    }

    public override void ShowFullInfo()
    {
        var oppoFullInfo = RoomSettings.I.UserInfos.FirstOrDefault(_ => _.Id == Id);
        FullUserView.Show(oppoFullInfo ?? Repository.I.PersonalFullInfo);
    }

    public class Manager
    {
        public static Manager I;
        public Manager()
        {
            I = this;
        }

        private async UniTask<RoomUserView> Create(int place, MinUserInfo minUserInfo)
        {
            var view = (await Addressables.InstantiateAsync($"roomUserView{place}", RoomReferences.I.Canvas)).GetComponent<RoomUserView>();

            view.Init(minUserInfo);

            return view;
        }

        public List<RoomUserView> RoomUserViews { get; set; }

        public async void Init()
        {
            RoomUserViews = new List<RoomUserView>();

            var oppoPlaceCounter = 1;

            for (int i = 0; i < RoomSettings.I.UserInfos.Count; i++)
            {
                var placeIndex = i == RoomSettings.I.MyTurn ? 0 : oppoPlaceCounter++;

                RoomUserViews.Add(await Create(placeIndex, RoomSettings.I.UserInfos[i]));
            }
        }
    }
}