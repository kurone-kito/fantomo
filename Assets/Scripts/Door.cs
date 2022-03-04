
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

/// <summary>ドア制御用ロジック。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Door : UdonSharpBehaviour
{
    /// <value>自分自身による施錠における、表示文字色。</value>
    private readonly Color myLockColor =
        new Color(0f, 0.5f, 1f, 0.5f);

    /// <value>他プレイヤーによる施錠における、表示文字色。</value>
    private readonly Color enemyLockColor =
        new Color(1f, 0.2f, 0.5f);

    /// <value>ドア施錠・解錠オブジェクト。</value>
    public GameObject lockSwitch = null;

    /// <value>ドア施錠状態の表示を持つオブジェクト。</value>
    public GameObject lockedText = null;

    /// <value>ドア施錠・解錠を継続可能な領域。</value>
    public Collider lockableArea = null;

    /// <value>隣接状態の表示を持つオブジェクト。</value>
    public GameObject neighborText = null;

    /// <value>ドア開放オブジェクト。</value>
    public GameObject openSwitch = null;

    /// <value>
    /// <para>ドアを施錠したユーザーの ID。</para>
    /// <para>ID が無効時は、施錠していない状態。</para>
    /// </value>
    [UdonSynced]
    public int lockedUser = 0;

    /// <summary>
    /// すべてのプレイヤーに対し、<seealso cref="InnerOpenDoor"/>
    /// を実行するようイベントを送信します。
    /// </summary>
    public void OpenDoor()
    {
        this.SendCustomNetworkEvent(
            NetworkEventTarget.All, nameof(InnerOpenDoor));
    }

    /// <summary>
    /// <para>ドアを開くと共に、1.5 秒後に閉じる予約をします。</para>
    /// <para>また同時に、ドア開閉スイッチを一時的に無効化します。</para>
    /// </summary>
    public void InnerOpenDoor()
    {
        this.setInteractive(false);
        this.updateInteraction();
        this.playAnimationToOpen();
        this.SendCustomEventDelayedSeconds(nameof(CloseDoor), 1.5f);
    }

    /// <summary>
    /// ドアを閉じると共に、0.5 秒後にドア開閉スイッチを有効化します。
    /// </summary>
    public void CloseDoor()
    {
        this.playAnimationToClose();
        this.SendCustomEventDelayedSeconds(
            nameof(EnableInteractive), 0.5f);
    }

    /// <summary>ドアの施錠状態を切り替えます。</summary>
    public void ToggleLock()
    {
        this.setLock(!this.isLocked());
        this.updateInteraction();
        if (!this.isLocked())
        {
            this.OpenDoor();
        }
        this.updateLockSwitchText();
        this.updateView();
    }

    /// <summary>スイッチ類を有効化します。</summary>
    public void EnableInteractive()
    {
        this.setInteractive(true);
    }

    /// <summary>
    /// 同期データを受領・適用した後に呼び出す、コールバック。
    /// </summary>
    public override void OnDeserialization()
    {
        this.updateInteraction();
        this.updateLockSwitchText();
        this.updateView();
    }

    /// <summary>ドアが施錠しているかどうかを取得します。</summary>
    /// <returns>ドアが施錠している場合、<c>true</c>。</returns>
    private bool isLocked()
    {
        var player = VRCPlayerApi.GetPlayerById(this.lockedUser);
        return player != null && player.IsValid();
    }

    /// <summary>
    /// 自分自身でドアを施錠しているかどうかを取得します。
    /// </summary>
    /// <returns>
    /// 施錠者がローカルプレイヤー、つまり自分自身である場合、<c>true</c>。
    /// </returns>
    private bool isMyLock()
    {
        var player = VRCPlayerApi.GetPlayerById(this.lockedUser);
        return player != null && player.isLocal;
    }

    /// <summary>施錠の有無を設定します。</summary>
    /// <param name="isLock">施錠するかどうか。</param>
    private void setLock(bool isLock)
    {
        this.changeOwner();
        this.lockedUser = isLock ? Networking.LocalPlayer.playerId : 0;
        this.RequestSerialization();
    }

    /// <summary>ドア閉鎖のアニメーションを再生します。</summary>
    private void playAnimationToClose()
    {
        var animator = this.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isOpen", false);
        }
    }

    /// <summary>ドア開放のアニメーションを再生します。</summary>
    private void playAnimationToOpen()
    {
        var animator = this.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isOpen", true);
        }
    }

    /// <summary>操作の有効・無効を設定します。</summary>
    /// <param name="isInteractive">操作が有効であるかどうか。</param>
    private void setInteractive(bool isInteractive)
    {
        if (this.lockSwitch != null)
        {
            this.lockSwitch.SetActive(isInteractive);
        }
        if (this.openSwitch != null)
        {
            this.openSwitch.SetActive(isInteractive);
        }
    }

    /// <summary>施錠操作が可能であるかどうかを設定します。</summary>
    /// <param name="enabled">操作が有効であるかどうか。</param>
    private void setLockSwitchEnabled(bool enabled)
    {
        if (this.lockSwitch != null)
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
    private void setOpenSwitchEnabled(bool enabled)
    {
        if (this.openSwitch != null)
        {
            var openSwitch = this.openSwitch.GetComponent<DoorSwitch>();
            if (openSwitch)
            {
                openSwitch.DisableInteractive = !enabled;
            }
        }
    }

    /// <summary>各種スイッチの有効・無効状態を更新します。</summary>
    private void updateInteraction()
    {
        this.setOpenSwitchEnabled(!this.isLocked());
    }

    /// <summary>施錠スイッチのラベルを更新します。</summary>
    private void updateLockSwitchText()
    {
        if (this.lockSwitch != null)
        {
            var lockSwitch =
                (UdonBehaviour)this.lockSwitch.GetComponent(
                    typeof(UdonBehaviour));
            if (lockSwitch != null)
            {
                lockSwitch.InteractionText =
                    this.isLocked() ? "解錠して開く" : "施錠";
            }
        }
    }

    /// <summary>表示状態を更新します。</summary>
    private void updateView()
    {
        if (this.lockedText != null)
        {
            this.lockedText.SetActive(this.isLocked());
            var lockedText = this.lockedText.GetComponent<Text>();
            if (lockedText != null)
            {
                lockedText.color =
                    this.isMyLock() ? myLockColor : enemyLockColor;
            }
        }
    }

    /// <summary>オブジェクトオーナーを奪取・変更します。</summary>
    private void changeOwner()
    {
        var player = Networking.LocalPlayer;
        if (!Networking.IsOwner(player, this.gameObject))
        {
            Networking.SetOwner(player, this.gameObject);
        }
    }
}
