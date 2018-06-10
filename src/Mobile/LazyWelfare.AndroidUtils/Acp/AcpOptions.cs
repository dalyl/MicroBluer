using System;

namespace LazyWelfare.AndroidUtils.Acp
{
	/// <summary>
	/// Created by hupei on 2016/4/26.
	/// </summary>
	public sealed class AcpOptions
	{
		private string mRationalMessage;
		private string mDeniedMessage;
		private string mDeniedCloseBtn;
		private string mDeniedSettingBtn;
		private string mRationalBtn;
		private string[] mPermissions;
		private bool mDialogCancelable;
		private bool mDialogCanceledOnTouchOutside;

		private AcpOptions(Builder builder)
		{
			mRationalMessage = builder.mRationalMessage;
			mDeniedMessage = builder.mDeniedMessage;
			mDeniedCloseBtn = builder.mDeniedCloseBtn;
			mDeniedSettingBtn = builder.mDeniedSettingBtn;
			mRationalBtn = builder.mRationalBtn;
			mPermissions = builder.mPermissions;
			mDialogCancelable = builder.dialogCancelable;
			mDialogCanceledOnTouchOutside = builder.dialogCanceledOnTouchOutside;
		}

		public string RationalMessage
		{
			get
			{
				return mRationalMessage;
			}
		}

		public string DeniedMessage
		{
			get
			{
				return mDeniedMessage;
			}
		}

		public string DeniedCloseBtn
		{
			get
			{
				return mDeniedCloseBtn;
			}
		}

		public string DeniedSettingBtn
		{
			get
			{
				return mDeniedSettingBtn;
			}
		}

		public string RationalBtnText
		{
			get
			{
				return mRationalBtn;
			}
		}

		public string[] Permissions
		{
			get
			{
				return mPermissions;
			}
		}

		public bool DialogCancelable
		{
			get
			{
				return mDialogCancelable;
			}
		}

		public bool DialogCanceledOnTouchOutside
		{
			get
			{
				return mDialogCanceledOnTouchOutside;
			}
		}

		public class Builder
		{
			internal const string DEF_RATIONAL_MESSAGE = "此功能需要您授权，否则将不能正常使用";
			internal const string DEF_DENIED_MESSAGE = "您拒绝权限申请，此功能将不能正常使用，您可以去设置页面重新授权";
			internal const string DEF_DENIED_CLOSE_BTN_TEXT = "关闭";
			internal const string DEF_DENIED_SETTINGS_BTN_TEXT = "设置权限";
			internal const string DEF_RATIONAL_BTN_TEXT = "我知道了";
			internal string mRationalMessage = DEF_RATIONAL_MESSAGE;
			internal string mDeniedMessage = DEF_DENIED_MESSAGE;
			internal string mDeniedCloseBtn = DEF_DENIED_CLOSE_BTN_TEXT;
			internal string mDeniedSettingBtn = DEF_DENIED_SETTINGS_BTN_TEXT;
			internal string mRationalBtn = DEF_RATIONAL_BTN_TEXT;
			internal string[] mPermissions;
			internal bool dialogCancelable = false;
			internal bool dialogCanceledOnTouchOutside = false;

			/// <summary>
			/// 申请权限理由框提示语
			/// </summary>
			/// <param name="rationalMessage">
			/// @return </param>
			public virtual Builder setRationalMessage(string rationalMessage)
			{
				mRationalMessage = rationalMessage;
				return this;
			}

			/// <summary>
			/// 申请权限理由框按钮
			/// </summary>
			/// <param name="rationalBtnText">
			/// @return </param>
			public virtual Builder setRationalBtn(string rationalBtnText)
			{
				this.mRationalBtn = rationalBtnText;
				return this;
			}

			/// <summary>
			/// 拒绝框提示语
			/// </summary>
			/// <param name="deniedMessage">
			/// @return </param>
			public virtual Builder setDeniedMessage(string deniedMessage)
			{
				mDeniedMessage = deniedMessage;
				return this;
			}

			/// <summary>
			/// 拒绝框关闭按钮
			/// </summary>
			/// <param name="deniedCloseBtnText">
			/// @return </param>
			public virtual Builder setDeniedCloseBtn(string deniedCloseBtnText)
			{
				this.mDeniedCloseBtn = deniedCloseBtnText;
				return this;
			}

			/// <summary>
			/// 拒绝框跳转设置权限按钮
			/// </summary>
			/// <param name="deniedSettingText">
			/// @return </param>
			public virtual Builder setDeniedSettingBtn(string deniedSettingText)
			{
				this.mDeniedSettingBtn = deniedSettingText;
				return this;
			}

			/// <summary>
			/// 需要申请的权限
			/// </summary>
			/// <param name="mPermissions"> <seealso cref="android.Manifest.permission android.Manifest.permission"/>
			/// @return </param>
			public virtual Builder SetPermissions(params string[] mPermissions)
			{
				this.mPermissions = mPermissions;
				return this;
			}

			public virtual Builder setDialogCancelable(bool dialogCancelable)
			{
				this.dialogCancelable = dialogCancelable;
				return this;
			}

			public virtual Builder setDialogCanceledOnTouchOutside(bool dialogCanceledOnTouchOutside)
			{
				this.dialogCanceledOnTouchOutside = dialogCanceledOnTouchOutside;
				return this;
			}

			public virtual AcpOptions Build()
			{
				if (this.mPermissions == null || this.mPermissions.Length == 0)
				{
					throw new System.ArgumentException("mPermissions no found...");
				}
				return new AcpOptions(this);
			}

            public object setPermissions(object wRITE_EXTERNAL_STORAGE, object rEAD_PHONE_STATE, object sEND_SMS)
            {
                throw new NotImplementedException();
            }
        }
	}

}