
using UdonSharp;

/// <summary>定数一覧。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Constants : UdonSharpBehaviour
{
    /// <summary>-Y の方角を示す定数。</summary>
    private const int _DIR_N = 0;

    /// <summary>-Y の方角を示す定数。</summary>
    public int DIR_N => _DIR_N;

    /// <summary>+X の方角を示す定数。</summary>
    private const int _DIR_E = 1;

    /// <summary>+X の方角を示す定数。</summary>
    public int DIR_E => _DIR_E;

    /// <summary>-X の方角を示す定数。</summary>
    private const int _DIR_W = 2;

    /// <summary>-X の方角を示す定数。</summary>
    public int DIR_W => _DIR_W;

    /// <summary>+Y の方角を示す定数。</summary>
    private const int _DIR_S = 3;

    /// <summary>+Y の方角を示す定数。</summary>
    public int DIR_S => _DIR_S;

    /// <summary>方角を示す定数の最大値。</summary>
    private const int _DIR_MAX = 4;

    /// <summary>方角を示す定数の最大値。</summary>
    public int DIR_MAX => _DIR_MAX;

    /// <summary>解錠状態を示すフラグ定数。</summary>
    private const int _LOCKSTATE_UNLOCKED = 0;

    /// <summary>解錠状態を示すフラグ定数。</summary>
    public int LOCKSTATE_UNLOCKED => _LOCKSTATE_UNLOCKED;

    /// <summary>自プレイヤーによる施錠状態を示すフラグ定数。</summary>
    private const int _LOCKSTATE_LOCKED_BY_ME = 1;

    /// <summary>自プレイヤーによる施錠状態を示すフラグ定数。</summary>
    public int LOCKSTATE_LOCKED_BY_ME => _LOCKSTATE_LOCKED_BY_ME;

    /// <summary>他プレイヤーによる施錠状態を示すフラグ定数。</summary>
    private const int _LOCKSTATE_LOCKED_BY_EMEMY = 2;

    /// <summary>他プレイヤーによる施錠状態を示すフラグ定数。</summary>
    public int LOCKSTATE_LOCKED_BY_EMEMY => _LOCKSTATE_LOCKED_BY_EMEMY;

    /// <summary>施錠状態の固定を示すフラグ定数。</summary>
    private const int _LOCKSTATE_DISABLED = 4;

    /// <summary>施錠状態の固定を示すフラグ定数。</summary>
    public int LOCKSTATE_DISABLED => _LOCKSTATE_DISABLED;

    /// <summary>鍵の設置数。</summary>
    private const int _NUM_KEYS = 9;

    /// <summary>鍵の設置数。</summary>
    public int NUM_KEYS => _NUM_KEYS;

    /// <summary>地雷の設置数。</summary>
    private const int _NUM_MINES = 9;

    /// <summary>地雷の設置数。</summary>
    public int NUM_MINES => _NUM_MINES;

    /// <summary>最大エントリー可能数。</summary>
    private const int _NUM_PLAYERS = 3;

    /// <summary>最大エントリー可能数。</summary>
    public int NUM_PLAYERS => _NUM_PLAYERS;

    /// <summary>ゲームフィールドの X 軸における部屋数。</summary>
    private const int _ROOMS_WIDTH = 8;

    /// <summary>ゲームフィールドの X 軸における部屋数。</summary>
    public int ROOMS_WIDTH => _ROOMS_WIDTH;

    /// <summary>扉を削除して、袋小路を形成する比率。</summary>
    private const float _ROOM_REMOVE_DOOR_RATE = 0.01f;

    /// <summary>扉を削除して、袋小路を形成する比率。</summary>
    public float ROOM_REMOVE_DOOR_RATE => _ROOM_REMOVE_DOOR_RATE;

    /// <summary>部屋の大きさ。</summary>
    private const float _ROOM_SIZE = 10f;

    /// <summary>部屋の大きさ。</summary>
    public float ROOM_SIZE => _ROOM_SIZE;

    /// <summary>ゲームフィールドにおける部屋数。</summary>
    private const int _NUM_ROOMS = _ROOMS_WIDTH * _ROOMS_WIDTH;

    /// <summary>ゲームフィールドにおける部屋数。</summary>
    public int NUM_ROOMS => _NUM_ROOMS;

    /// <summary>
    /// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>する、
    /// オブジェクト一覧の要素数。
    /// </summary>
    /// <remarks>
    /// 末尾の 2 は、MirrorSystem と EntrySystem を指している。
    /// </remarks>
    private const int _NUM_INSTANTIATES = _NUM_KEYS + _NUM_MINES + _NUM_ROOMS + 2;

    /// <summary>
    /// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>する、
    /// オブジェクト一覧の要素数。
    /// </summary>
    public int NUM_INSTANTIATES => _NUM_INSTANTIATES;
}
