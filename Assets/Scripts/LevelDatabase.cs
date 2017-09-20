using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game.tool;
using LitJson;
namespace game
{
    public struct stru
    {
        public string []manname;
        public int age;

         
    }
    public class LevelDatabase : Singletun<LevelDatabase>
    {
        public int colCount;
        public int rowCount;

        // 仅在首次调用 Update 方法之前调用 Start
        void Start()
        {
            //var Text =  Resources.Load<TextAsset>("Test");
            // Debug.Log(Text.text);
            //JsonData jd =  JsonMapper.ToObject(Text.text);
            // if (jd["List"].IsArray)
            // {
            //     foreach (JsonData token in jd["List"])
            //     {
            //         Debug.Log(toen["age"].ToString());
            //        k Debug.Log(token["manname"][0].ToString());
            //         Debug.Log(token["manname"][1].ToString());
            //     }
            // }
            // Debug.Log(JsonMapper.ToJson(jd));
            //List<string> testList = new List<string>() { "asd", "qwe" };
            //string str = JsonMapper.ToJson(testList);

            //List<string> _testList = JsonMapper.ToObject<List<string>>(str);
            //foreach (var token in _testList)
            //{
            //    Debug.Log(token);
            //}
            //var test2 =  JsonMapper.ToObject<List<stru>>("[{\"manname\":[\"Bill\",\"George\"],\"age\":18},{\"manname\":[\"Gates\",\"Bush\"],\"age\":19}]");
            // Debug.Log(test2);
            //List<stru> test3 = new List<stru>();
            //stru x1;
            //x1.age = 10;
            //x1.manname = new string[2];
            //x1.manname[0] = "asdadsad";
            //x1.manname[1] = "qweqrqrqr";
            //test3.Add(x1);
            //// //持久化
            //string sav = JsonMapper.ToJson(test3);

            //PlayerPrefs.SetString("SAVE", sav);

            //PlayerPrefs.Save();
            string sav = PlayerPrefs.GetString("SAVE");
            List<stru> test3 = JsonMapper.ToObject<List<stru>>(sav);
            foreach (var token in test3)
            {
                Debug.Log(token.age + token.manname[0]);
            }
        }
     

        // Update is called once per frame
        void Update()
        {

        }
    }
}
