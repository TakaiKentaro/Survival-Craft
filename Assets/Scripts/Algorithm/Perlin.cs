using Unity.VisualScripting;
using UnityEditor;

class Perlin
{
    /// <summary>
    /// x、y、z座標、オクターブ数、および進行度を取得し、それらを使用してPerlinノイズを生成
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="octaves"></param>
    /// <param name="persistence"></param>
    /// <returns></returns>
    public double OctavePerlin(double x, double y, double z, int octaves, double persistence)
    {
        double total = 0; // 合計値
        double frequency = 1; // 周波数
        double amplitude = 1; // 振れ幅
        
        // x、y、z座標に対してPerlinノイズを生成。その後、生成されたPerlinノイズの値がamplitudeによって乗算され、その結果がtotalに加算
        // 次に、amplitudeがpersistenceによって乗算され、frequencyは2倍になる。
        // よって、より高周波数のPerlinノイズが生成され、より小さなアンプリフィケーションが適用される。
        // オクターブのような概念を実現し、より詳細なPerlinノイズを生成する
        for (int i = 0; i < octaves; i++)
        {
            total += perlin(x * frequency, y * frequency, z * frequency) * amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        return total;
    }

    /// <summary>
    /// Perlinノイズの計算に使用される0から255までのすべての数字をランダムに並べた数字の配列
    /// </summary>
    private static readonly int[] permutation =
    {
        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30,
        69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
        94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136,
        171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
        60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161,
        1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
        164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126,
        255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
        119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253,
        19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
        238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
        181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
        222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
    };

    /// <summary>
    /// permutation配列のコピーで、オーバーフローを避ける用に使用
    /// </summary>
    private static readonly int[] p;

    public int repeat;

    public Perlin(int repeat = -1)
    {
        this.repeat = this.repeat;
    }

    /// <summary>
    /// x、y、z座標を使用してPerlinノイズを生成
    /// </summary>
    static Perlin()
    {
        p = new int[512];
        for (int x = 0; x < 512; x++)
        {
            p[x] = permutation[x % 512];
        }
    }

    /// <summary>
    /// repeat "変数が0より大きいかどうかをチェックし、大きい場合はx、y、zの値を "repeat "で割った余りを計算し、その結果をそれぞれx、y、zに割り戻している
    /// 座標が特定の範囲に収まるように包み込む
    /// 効果的にノイズパターンを繰り返させることができる
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public double perlin(double x, double y, double z)
    {
        if (repeat > 0)
        {
            x = x * repeat;
            y = y * repeat;
            z = z * repeat;
        }

        // 点 (x, y, z) が位置する「単位立方体」を計算するために使用
        // 点 (x, y, z) におけるパーリンノイズ値を生成するために使用される,並べ替え配列の値を検索するためのインデックスとして使用
        // 並べ換え配列は256の要素（0-255）しかないため、ビット演算子&255を適用することで、座標の整数表現の最後の8ビットを効果的に取得することができる
        // これでこれらの座標をインデックスとして使って並べ換え配列の中の値を調べることができる
        int xi = (int)x & 255;
        int yi = (int)y & 255;
        int zi = (int)z & 255;

        // 座標の整数部分を元の座標から引くことによって、座標の小数部分（x, y, z）を計算
        // まず x,y,z を int にキャストし、それぞれ x,y,z から引き算する
        // 変数 xf,yf,zfは座標(x, y, z)の小数部分を含み点(x,y,z)における最終的なパーリンノイズ値を生成するための補間処理に使用される
        // 補間は、2つの既知の値間の値を推定するために使用される手法で、この場合は、点 (x, y, z) が位置する単位立方体の角におけるパーリンノイズ値である
        // xf,yf,zfに格納された値は、点(x,y,z)の値と次の単位立方体に位置する点(x+1,y+1,z+1)の値の間の滑らかな遷移を計算するために使用される
        double xf = x - (int)x;
        double yf = y - (int)y;
        double zf = z - (int)z;

        // 単位立方体の異なる部分におけるノイズ値の遷移を滑らかにするために、座標の小数部分(xf,yf,zf)にfade()関数を適用する
        // 単位立方体の角におけるノイズ値間の遷移を滑らかにすることで、補間処理を滑らかにするために使用される
        // 座標の分数部分（xf, yf, zf）にそれぞれ適用され、変数 u, v, w となる
        double u = fade(xf);
        double v = fade(yf);
        double w = fade(zf);

        // 単位立方体内の点 (x, y, z) の並べ替えられたインデックスを計算、これらの並べ替えインデックスは、点 (x, y, z) におけるノイズ値を生成するために使用します
        // xi、yi、zi座標（先に計算）をインデックスとして使用し、並べ替え配列の値を検索し、permutation[a]の値がziに追加され、インデックスaaが得られる
        // この処理を繰り返し、aに1を加え、ziを加えてインデックスabを得る
        // 次に、この処理を繰り返して、xiに1を加え、yiを加えて添字bを得て、この処理を再び繰り返し、permutation[b]とpermutation[b+1]にそれぞれziを加えてインデックスbaとbbを得る
        // 並べ替えられたインデックスaa、ab、ba、bbは、点（x、y、z）におけるノイズ値を計算するために使用される並べ替え配列の値を調べるために使用する
        int a = p[xi] + yi;
        int aa = p[a] + zi;
        int ab = p[a + 1] + zi;
        int b = p[xi + 1] + yi;
        int ba = p[b] + zi;
        int bb = p[b + 1] + zi;
        
        // 与えられた「u」と「v」の値に基づいて2つの値（x1、x2）と（y1、y2）の間を補間するために、lerp（線形補間）関数を使用する
        // ノイズ空間における与えられた点の勾配を計算するために使用されるgrad関数を使用し、関数のp（2倍の並べ換え配列）、xf、yf、zfの値を入力として取り込みます
        // パーリンノイズを生成するために「フラクショナルブラウン運動」（fBm）と呼ばれる技術を使用している(fBmは複数のオクターブのノイズを組み合わせて、より複雑なノイズパターンを生成する技術)
        // 点のz値に対して2つのノイズ値（y1、y2）を生成し、「v」値に基づいてそれらの間を補間することにより、2Dのノイズパターンを生成している。
        // また、点のxとyの値に対して2つのノイズ値(x1, x2)を生成し、それらの間を "u"の値に基づいて補間している
        double x1, x2, y1, y2;
        x1 = lerp(grad(p[aa], xf, yf, zf),
            grad(p[ba], xf - 1, yf, zf), u);
        x2 = lerp(grad(p[ab], xf, yf - 1, zf),
            grad(p[bb], xf - 1, yf - 1, zf),
            u);
        y1 = lerp(x1, x2, v);
        
        x1 = lerp(grad(p[aa + 1], xf, yf, zf - 1),
            grad(p[ba + 1], xf - 1, yf, zf - 1), u);
        x2 = lerp(grad(p[ab + 1], xf, yf - 1, zf - 1),
            grad(p[bb + 1], xf - 1, yf - 1, zf - 1), u);
        y2 = lerp(x1, x2, v);

        // 与えられたポイント(x, y, z)で生成されたパーリンノイズの最終値を返す
        // 再びlerp関数を使用して、以前に生成された2つのノイズ値（y1、y2）の間を「w」値に基づいて補間する
        // パーリンノイズアルゴリズムが1より大きい値を返すことがあるので、値を[0,1]の範囲に正規化する
        return (lerp(y1, y2, w) + 1) / 2;
    }
    
    /// <summary>
    /// 与えられたハッシュ値に基づいて、3次元空間内の点の勾配を決定する
    /// ハッシュ値は、並べ替え配列の中からあらかじめ決められた勾配ベクトルを探す
    /// 勾配ベクトルと点の座標（x, y, z）の内積を計算
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static double grad(int hash, double x, double y, double z)
    {
        int h = hash & 15; // ハッシュ化された値の最初の4ビットを取得
        double u = h < 8 ? x : y; // ハッシュの最上位ビット(MSB)が0であれば、u = xとし、そうでなければyとする
        double v; // KenPerlinのオリジナルの実装では、これは別の条件演算子だったものを読みやすく拡張

        // ドットプロダクトの計算で使用する座標（x,y,z）のいずれかを選択する
        if (h < 4) // vの値はyに設定
        {
            v = y;
        }
        else if (h == 12 || h == 14) // vの値はxに設定
        {
            v = x;
        }
        else // vの値はzに設定
        {
            v = z;
        }
        
        // 最後の2ビットでuとvが正か負かを判断し、それらの足した値を返す
        return ((h&1) == 0 ? u : -u)+((h&2) == 0 ? v : -v); 
    }

    /// <summary>
    /// 単位立方体の角のノイズ値の間の遷移を滑らかにすることで、補間処理を滑らかにするために使用される
    /// 入力値(t)を滑らかなエルミート曲線に写像し、単位立方体の異なる部分におけるノイズ値の間の遷移を滑らかにするもの
    /// 6t^5 - 15t^4 + 10t^3 の多項式で定義され、補間処理で重み付け係数として使用される0から1への滑らかな遷移を作成する
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static double fade(double t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    /// <summary>
    /// 与えられた分数 "x" で線形補間するlerpメソッド
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static double lerp(double a, double b, double x)
    {
        return a + x * (b - a);
    }
}