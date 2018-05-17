using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace LazyWelfare.AndroidMobile
{
    [Activity]
    public class BulbActivity : Activity
    {
        Handler handler { get; set; }

        public BulbActivity()
        {
           // handler = new Handler(msg =>
           //{
           //    var list = new List<string>();
           //     //因为是简单版，我就直接把所有的图片都存到一个list里用来显示了
           //    foreach (var pictureBean in beanList)
           //    {
           //        foreach (var s in pictureBean.getImageList())
           //        {
           //            list.Add(s);
           //        }
           //    }
           //     //adapter.setList(list);
           //     //adapter.notifyDataSetChanged();
           //     // adapter.setmOnClick(
           // });
        }

      

        public static event Action<string[]> OnSelectCompleted;
     
        //public void itemOnClick(int position)
        //{//点击事件就是把被点击的图片的绝对路径返回去
        //    Intent intent = new Intent();
        //    intent.PutExtra("image", list.get(position));
        //    SetResult(RESULT_OK, intent);
        //    Finish();
        //}

        private List<PictureBean> beanList;
        //private void getImages()
        //{
        //    Task.Factory.StartNew(() =>
        //    {

        //        Uri imageUri = MediaStore.Images.Media.ExternalContentUri;//这个Uri代表要查询的数据库名称加上表的名称

        //        //这个就是查询操作了吧（cursor,游标，听起来很熟悉，老师绝对讲过，不过原谅我数据库这一块渣到爆炸）
        //        var cursor = this.ContentResolver.Query(imageUri, null, MediaStore.Images.Media.InterfaceConsts.MimeType + "=? or " + MediaStore.Images.Media.InterfaceConsts.MimeType + "=?", new string[] { "image/jpeg" }, MediaStore.Images.Media.InterfaceConsts.DateModified);

        //        if (cursor == null)
        //        {
        //            return;
        //        }

        //        List<string> pathList = new List<string>();//同一个文件夹下的图片的集合
        //        string mParentPath = "";//所在文件夹的绝对路径
        //        while (cursor.MoveToNext())
        //        {
        //            string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data));//图片的绝对路径


        //            FileInfo file = new FileInfo(path).getParentFile();
        //            string parentPath = file.getAbsolutePath();//文件夹的绝对路径

        //            if (cursor.IsFirst)
        //            {//如果是第一个图片
        //                pathList.Add(path);//直接加到图片集合里
        //                mParentPath = parentPath;//文件夹的绝对路径 等于 我们获取到的这张图片的 文件夹的绝对路径
        //            }
        //            else if (mParentPath.Equals(parentPath))
        //            {//如果 新的图片的文件夹的绝对路径 跟上一张图片的文件夹的绝对路径相同， 说明两张图片在同一个文件夹下
        //                pathList.Add(path);//继续填到集合里
        //            }
        //            else
        //            {//说明 新的图片跟 上一张图片不在同一个文件夹 下
        //                List<string> list = new List<string>();
        //                list.AddRange(pathList); //用一个新的list 加载 之前一个文件夹下的所有图片(这里暂时想不到更好的办法了)
        //                pathList.Clear();
        //                pathList.Add(path); //旧的list 清空之后呢， 开始加载 新的文件夹下的 第一张图片(仔细看一看就知道逻辑了，代码写的确实比较烂，哈哈哈)
        //                PictureBean bean = new PictureBean();//我们之前写好的模型
        //                bean.setNumber(list.Count()); //数量就是list的数量
        //                bean.setFirstImageUrl(list.First()); //第一张图片
        //                bean.setImageList(list);//图片集合
        //                bean.setParentPath(mParentPath);//所在文件夹的绝对路径
        //                beanList.Add(bean);//把模型放到步骤3定义好的list
        //                mParentPath = parentPath; //文件夹绝对路径 变成了 新的文件夹路径
        //            }
        //        }
        //        //到此为止，喜大普奔，所有的图片都已经获取到了，接下来直接显示出来(想做复杂的呢可以参考微信，我正在参考中...)
        //        handler.SendEmptyMessage(1);

        //    });
        //}


    }

    public class PictureBean
    {//我的想法是以每一个放图片的文件夹为一个模型

        private string name;//文件夹的名字，以后会用到，具体用处可以先打开微信的图片选择器，点左下角视频和图片，弹出一个popupwindow，每一个item都有一个name，他这个name应该是图片父目录的文件名，我这个name也是这个意思，不过这篇博客应该暂时用不到
        private int number;//这个文件夹下图片的数量
        private string firstImageUrl;//这个文件夹下的第一张图片
        private List<string> imageList;//这个文件夹下的图片集合
        private string parentPath;//这个文件夹的绝对路径

        public string getParentPath()
        {
            return parentPath;
        }

        public void setParentPath(string parentPath)
        {
            this.parentPath = parentPath;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public int getNumber()
        {
            return number;
        }

        public void setNumber(int number)
        {
            this.number = number;
        }

        public string getFirstImageUrl()
        {
            return firstImageUrl;
        }

        public void setFirstImageUrl(string firstImageUrl)
        {
            this.firstImageUrl = firstImageUrl;
        }

        public List<string> getImageList()
        {
            return imageList;
        }

        public void setImageList(List<string> imageList)
        {
            this.imageList = imageList;
        }

    }
 

}
