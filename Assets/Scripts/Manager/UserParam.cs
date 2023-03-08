using System;

/// <summary>
/// 共有する情報
/// </summary>
[Serializable]
public struct UserParam
{
    public string Name; //ユーザーの名前
    public int Rank; //ユーザーのレート   

    public string[] GetPropertiesString()
    {
        //下で定義するHashTableのキーと同じ値を入れる
        return new string[]
        {
            "Name",
            "Rank"
        };
    }

    /// <summary>
    /// 文字列のキーと、Photonで通信できるデータ型の値のペア
    /// </summary>
    /// <returns></returns>
    public ExitGames.Client.Photon.Hashtable CreateHashTable()
    {
        ExitGames.Client.Photon.Hashtable roomProp = new ExitGames.Client.Photon.Hashtable();

        //共有したい情報を入れる
        roomProp["Name"] = Name;
        roomProp["Rank"] = Rank;

        return roomProp;
    }

    public void UpdateHashTable(ExitGames.Client.Photon.Hashtable table)
    {
        ExitGames.Client.Photon.Hashtable roomProp = new ExitGames.Client.Photon.Hashtable();

        //上書きする情報を入れる
        Name = table["Name"].ToString();
        Rank = Int32.Parse(table["Rank"].ToString());
    }
};
