using System.Collections.Generic;

public static class WaitForSecondsCache
{   
    private static Dictionary<float, UnityEngine.WaitForSeconds> dic = new Dictionary<float, UnityEngine.WaitForSeconds>();

    public static UnityEngine.WaitForSeconds Wait(float sec)
    {
        if(!dic.ContainsKey(sec))
            dic[sec] = new UnityEngine.WaitForSeconds(sec);

        return dic[sec];
    }
}