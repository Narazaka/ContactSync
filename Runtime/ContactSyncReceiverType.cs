namespace Narazaka.VRChat.ContactSync
{
    public enum ContactSyncReceiverType
    {
        // toggle
        Constant,
        OnEnter,
        Proximity,
        // senderで無指定があるときと、receiverで抜けがある時がある（OnEnterとかでなんとかする？）
        // button
        [IString("Trigger", "トリガー")]
        Trigger,
        // on toggle / off toggle
        [IString("Toggle", "ON OFF")]
        Toggle,
        // toggle
        // ToggleAlways,
        // many toggles + empty toggle param:何個か 0はなにもしない固定のためインデックスから1減らした値を割り付ける
        [IString("Choose", "選択")]
        Choose,
        // many toggles param:何個か
        // ChooseAlways,
        // toggle + radial
        [IString("Radial", "無段階")]
        Radial,
        // radial
        // RadialAlways,
    }
}
