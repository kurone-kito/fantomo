
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

/// <summary>ドア制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Door : UdonSharpBehaviour
{
    /// <summary>ドア施錠・解錠オブジェクト。</summary>
    [SerializeField]
    private GameObject lockSwitch = null;

    /// <summary>ドア施錠・解錠を継続可能な領域。</summary>
    [SerializeField]
    private Collider lockableArea = null;

    /// <summary>ドア開放オブジェクト。</summary>
    [SerializeField]
    private GameObject openSwitch = null;

    /// <summary>ドア表示状態コントローラー。</summary>
    [SerializeField]
    private DoorStateViewController stateViewController = null;

    /// <summary>
    /// <para>ドアを施錠したユーザーの ID。</para>
    /// <para>ID が無効時は、施錠していない状態。</para>
    /// </summary>
    [NonSerialized]
    [UdonSynced]
    public int lockedUser = 0;

    /// <summary>ドア施錠・解錠をキャンセルするかどうか。</summary>
    private bool isLockCanceled = false;

    /// <summary>ドア施錠・解錠をキャンセルします。</summary>
    public void CancelLock()
    {
        isLockCanceled = true;
        if (stateViewController != null)
        {
            stateViewController.IgnoreProgress();
        }
    }

    /// <summary>
    /// すべてのプレイヤーに対し、<seealso cref="InnerOpenDoor"/>
    /// を実行するようイベントを送信します。
    /// </summary>
    public void OpenDoor()
    {
        SendCustomNetworkEvent(
            NetworkEventTarget.All, nameof(InnerOpenDoor));
    }

    /// <summary>
    /// <para>ドアを開くと共に、1.5 秒後に閉じる予約をします。</para>
    /// <para>また同時に、ドア開閉スイッチを一時的に無効化します。</para>
    /// </summary>
    public void InnerOpenDoor()
    {
        CancelLock();
        if (stateViewController != null)
        {
            stateViewController.StopProgress();
        }
        SetInteractive(false);
        UpdateInteraction();
        PlayAnimationToOpen();
        SendCustomEventDelayedSeconds(nameof(CloseDoor), 1.5f);
    }

    /// <summary>
    /// ドアを閉じると共に、0.5 秒後にドア開閉スイッチを有効化します。
    /// </summary>
    public void CloseDoor()
    {
        PlayAnimationToClose();
        SendCustomEventDelayedSeconds(
            nameof(EnableInteractive), 0.5f);
    }

    /// <summary>施錠・解錠の予約をします。</summary>
    public void ReserveToggleLock()
    {
        SetLockSwitchEnabled(false);
        var wait = IsLocked() && !IsMyLock() ? 5f : 1.5f;
        if (stateViewController != null)
        {
            stateViewController.StartProgress(wait);
        }
        isLockCanceled = false;
        SendCustomEventDelayedSeconds(
            nameof(ToggleLockWhenNotCanceled), wait);
    }

    /// <summary>
    /// <para>
    /// 途中キャンセルがなかった場合、ドアの施錠状態を切り替えます。
    /// </para>
    /// <para>
    /// <seealso cref="UdonSharpBehaviour.SendCustomEventDelayedSeconds"/>
    /// のキャンセルができないため、代替として実装した、Facade メソッドです。
    /// </para>
    /// </summary>
    public void ToggleLockWhenNotCanceled()
    {
        SetLockSwitchEnabled(true);
        if (!isLockCanceled)
        {
            ToggleLock();
        }
    }

    /// <summary>ドアの施錠状態を切り替えます。</summary>
    public void ToggleLock()
    {
        SetLock(!IsLocked());
        UpdateInteraction();
        if (!IsLocked())
        {
            OpenDoor();
        }
        UpdateLockSwitchText();
        UpdateView();
    }

    /// <summary>スイッチ類を有効化します。</summary>
    public void EnableInteractive()
    {
        SetInteractive(true);
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        UpdateInteraction();
        UpdateLockSwitchText();
        UpdateView();
    }

    /// <summary>ドアが施錠しているかどうかを取得します。</summary>
    /// <returns>ドアが施錠している場合、<c>true</c>。</returns>
    private bool IsLocked()
    {
        var player = VRCPlayerApi.GetPlayerById(lockedUser);
        return player != null && player.IsValid();
    }

    /// <summary>
    /// 自分自身でドアを施錠しているかどうかを取得します。
    /// </summary>
    /// <returns>
    /// 施錠者がローカルプレイヤー、つまり自分自身である場合、<c>true</c>。
    /// </returns>
    private bool IsMyLock()
    {
        var player = VRCPlayerApi.GetPlayerById(lockedUser);
        return player != null && player.isLocal;
    }

    /// <summary>施錠の有無を設定します。</summary>
    /// <param name="isLock">施錠するかどうか。</param>
    private void SetLock(bool isLock)
    {
        ChangeOwner();
        lockedUser = isLock ? Networking.LocalPlayer.playerId : 0;
        RequestSerialization();
    }

    /// <summary>ドア閉鎖のアニメーションを再生します。</summary>
    private void PlayAnimationToClose()
    {
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isOpen", false);
        }
    }

    /// <summary>ドア開放のアニメーションを再生します。</summary>
    private void PlayAnimationToOpen()
    {
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isOpen", true);
        }
    }

    /// <summary>操作の有効・無効を設定します。</summary>
    /// <param name="isInteractive">操作が有効であるかどうか。</param>
    private void SetInteractive(bool isInteractive)
    {
        if (lockSwitch != null)
        {
            lockSwitch.SetActive(isInteractive);
        }
        if (openSwitch != null)
        {
            openSwitch.SetActive(isInteractive);
        }
    }

    /// <summary>施錠操作が可能であるかどうかを設定します。</summary>
    /// <param name="enabled">操作が有効であるかどうか。</param>
    private void SetLockSwitchEnabled(bool enabled)
    {
        if (lockSwitch != null)
        {
            var lockSwitch = this.lockSwitch.GetComponent<DoorLock>();
            if (lockSwitch != null)
            {
                lockSwitch.DisableInteractive = !enabled;
            }
        }
    }

    /// <summary>開放操作が可能であるかどうかを設定します。</summary>
    /// <param name="enabled">操作が有効であるかどうか。</param>
    private void SetOpenSwitchEnabled(bool enabled)
    {
        if (openSwitch != null)
        {
            var openSwitch = this.openSwitch.GetComponent<DoorSwitch>();
            if (openSwitch)
            {
                openSwitch.DisableInteractive = !enabled;
            }
        }
    }

    /// <summary>各種スイッチの有効・無効状態を更新します。</summary>
    private void UpdateInteraction()
    {
        SetOpenSwitchEnabled(!IsLocked());
    }

    /// <summary>施錠スイッチのラベルを更新します。</summary>
    private void UpdateLockSwitchText()
    {
        if (lockSwitch != null)
        {
            var lockSwitch = this.lockSwitch.GetComponent<UdonBehaviour>();
            if (lockSwitch != null)
            {
                lockSwitch.InteractionText =
                    IsLocked() ? "解錠して開く" : "施錠";
            }
        }
    }

    /// <summary>表示状態を更新します。</summary>
    private void UpdateView()
    {
        if (stateViewController != null)
        {
            stateViewController.UpdateLockState(
                IsMyLock() ? stateViewController.LOCKED_BY_ME :
                IsLocked() ? stateViewController.LOCKED_BY_ENEMY :
                stateViewController.UNLOCKED);
        }
    }

    /// <summary>オブジェクトオーナーを奪取・変更します。</summary>
    private void ChangeOwner()
    {
        var player = Networking.LocalPlayer;
        if (!Networking.IsOwner(player, gameObject))
        {
            Networking.SetOwner(player, gameObject);
        }
    }
}
