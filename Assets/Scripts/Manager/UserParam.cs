using System;

/// <summary>
/// ���L������
/// </summary>
[Serializable]
public struct UserParam
{
    public string Name; //���[�U�[�̖��O
    public int Rank; //���[�U�[�̃��[�g   

    public string[] GetPropertiesString()
    {
        //���Œ�`����HashTable�̃L�[�Ɠ����l������
        return new string[]
        {
            "Name",
            "Rank"
        };
    }

    /// <summary>
    /// ������̃L�[�ƁAPhoton�ŒʐM�ł���f�[�^�^�̒l�̃y�A
    /// </summary>
    /// <returns></returns>
    public ExitGames.Client.Photon.Hashtable CreateHashTable()
    {
        ExitGames.Client.Photon.Hashtable roomProp = new ExitGames.Client.Photon.Hashtable();

        //���L��������������
        roomProp["Name"] = Name;
        roomProp["Rank"] = Rank;

        return roomProp;
    }

    public void UpdateHashTable(ExitGames.Client.Photon.Hashtable table)
    {
        ExitGames.Client.Photon.Hashtable roomProp = new ExitGames.Client.Photon.Hashtable();

        //�㏑�������������
        Name = table["Name"].ToString();
        Rank = Int32.Parse(table["Rank"].ToString());
    }
};
