namespace LazyWelfare.AndroidUtils.Acp
{
	using Context = Android.Content.Context;

	public class Acp
	{

		private static Acp mInstance;
		private AcpManager mAcpManager;

		public static Acp getInstance(Context context)
		{
			if (mInstance == null)
			{
				lock (typeof(Acp))
				{
					if (mInstance == null)
					{
						mInstance = new Acp(context);
					}
				}
			}
			return mInstance;
		}

		private Acp(Context context)
		{
			mAcpManager = new AcpManager(context.ApplicationContext);
		}

		/// <summary>
		/// 开始请求
		/// </summary>
		/// <param name="options"> </param>
		/// <param name="acpListener"> </param>
		public virtual void request(AcpOptions options, IAcpListener acpListener)
		{
			if (options == null)
			{
				new System.NullReferenceException("AcpOptions is null...");
			}
			if (acpListener == null)
			{
				new System.NullReferenceException("AcpListener is null...");
			}
			mAcpManager.Request(options, acpListener);
		}

		internal virtual AcpManager AcpManager
		{
			get
			{
				return mAcpManager;
			}
		}
	}

}