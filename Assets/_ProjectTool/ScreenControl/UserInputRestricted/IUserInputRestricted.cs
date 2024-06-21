//ユーザーの入力制限インターフェース
//MRコンテンツとタブレットコンテンツで必要な入力のブロックの要件が変わるため抽象化
public interface IUserInputRestricted
{
    void Block();
    void UnBlock();
    void ForcedUnBlock();
}