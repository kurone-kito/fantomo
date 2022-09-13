
using System;
using UdonSharp;

/// <summary>方角を示す定数一覧。</summary>
public enum DIR
{
    /// <summary>-Y の方角を示す定数。</summary>
    N = 0,
    /// <summary>+Y の方角を示す定数。</summary>
    S = 1,
    /// <summary>-X の方角を示す定数。</summary>
    W = 2,
    /// <summary>+X の方角を示す定数。</summary>
    E = 3,
    /// <summary>方角を示す定数の最大値。</summary>
    MAX = 4,
}

/// <summary>部屋ビットフラグ。</summary>
[Flags]
public enum ROOM_FLG
{
    /// <summary>部屋フラグにおける、-Y の方角を示すビット。</summary>
    DIR_N = 1 << DIR.N,
    /// <summary>部屋フラグにおける、+Y の方角を示すビット。</summary>
    DIR_S = 1 << DIR.S,
    /// <summary>部屋フラグにおける、-X の方角を示すビット。</summary>
    DIR_W = 1 << DIR.W,
    /// <summary>部屋フラグにおける、+X の方角を示すビット。</summary>
    DIR_E = 1 << DIR.E,
    /// <summary>部屋フラグにおける、全方位を示すビット。</summary>
    DIR_ALL = DIR_N | DIR_S | DIR_W | DIR_E,
    /// <summary>部屋フラグにおける、鍵所持を示すビット。</summary>
    HAS_KEY = 1 << 4,
    /// <summary>部屋フラグにおける、地雷所持を示すビット。</summary>
    HAS_MINE = 1 << 5,
    /// <summary>
    /// 部屋フラグにおける、スポーン地点所持を示すビット。
    /// </summary>
    HAS_SPAWN = 1 << 6,
    /// <summary>全アイテム所持の部屋を示すビット。</summary>
    HAS_ALL = HAS_KEY | HAS_MINE | HAS_SPAWN,
}

/// <summary>定数一覧。</summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Constants : UdonSharpBehaviour
{
    /// <summary>初期化処理におけるインターバル。</summary>
    public float LOAD_INTERVAL => 0.2f;

    /// <summary>解錠状態を示すフラグ定数。</summary>
    public int LOCK_STATE_UNLOCKED => 0;

    /// <summary>自プレイヤーによる施錠状態を示すフラグ定数。</summary>
    public int LOCK_STATE_LOCKED_BY_ME => 1;

    /// <summary>他プレイヤーによる施錠状態を示すフラグ定数。</summary>
    public int LOCK_STATE_LOCKED_BY_ENEMY => 2;

    /// <summary>施錠状態の固定を示すフラグ定数。</summary>
    public int LOCK_STATE_DISABLED => 4;

    /// <summary>鍵の設置数。</summary>
    public int NUM_KEYS => 9;

    /// <summary>地雷の設置数。</summary>
    public int NUM_MINES => 9;

    /// <summary>最大エントリー可能数。</summary>
    public int NUM_PLAYERS => 3;

    /// <summary>部屋フラグにおける、ビット数。</summary>
    public byte ROOM_BIT_MAX => 8;

    /// <summary>ゲームフィールドの X 軸における部屋数。</summary>
    /// <remarks>
    /// ビット演算の関係上、この値は 11 以下である必要があります。
    /// </remarks>
    public int ROOMS_WIDTH => 8;

    /// <summary>扉を削除して、袋小路を形成する比率。</summary>
    public float ROOM_REMOVE_DOOR_RATE => 0.01f;

    /// <summary>部屋の大きさ。</summary>
    public float ROOM_SIZE => 10f;

    /// <summary>ゲームフィールドにおける部屋数。</summary>
    /// <remarks>
    /// ビット演算の関係上、この値は 64 以下である必要があります。
    /// </remarks>
    public int NUM_ROOMS => ROOMS_WIDTH * ROOMS_WIDTH;

    /// <summary>
    /// <seealso cref="UdonSharpBehaviour.VRCInstantiate"/>する、
    /// オブジェクト一覧の要素数。
    /// </summary>
    /// <remarks>
    /// 末尾の 2 は、MirrorSystem と EntrySystem を指している。
    /// </remarks>
    public int NUM_INSTANTIATES => NUM_KEYS + NUM_MINES + NUM_ROOMS + 2;
}
