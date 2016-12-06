﻿using System;

namespace Qiniu
{
	/// <summary>
	/// Configuration.
	/// </summary>
	public class Configuration
	{
		/// <summary>
		/// The VERSION.
		/// </summary>
		public const String VERSION = "7.0.0";
		/**
     * 断点上传时的分块大小(默认的分块大小, 不建议改变)
     */
		public const int BLOCK_SIZE = 4 * 1024 * 1024;

		/**
     * 默认API服务器
     */
		public static String API_HOST = "http://api.qiniu.com";
		/**
     * 默认文件列表服务器
     */
		public static String RSF_HOST = "http://rsf.qbox.me";
		/**
     * 默认文件管理服务器
     */
		public static String RS_HOST = "http://rs.qbox.me";
		/**
     * 默认文件服务器
     */
		public static String IO_HOST = "http://iovip.qbox.me";
		/**
     * 默认上传服务器
     */
		public  String UP_HOST = "http://up.qiniu.com";
		/**
     * 备用上传服务器，当默认服务器网络链接失败时使用
     */
		public  String UP_HOST_BACKUP = "http://upload.qiniu.com";
		/**
     * 断点上传时的分片大小(可根据网络情况适当调整)
     */
		public  int CHUNK_SIZE = 256 * 1024;
		/**
     * 如果文件大小大于此值则使用断点上传, 否则使用Form上传
     */
		public int PUT_THRESHOLD = BLOCK_SIZE;
		/**
     * 连接超时时间(默认10s)
     */
		public const int DEFAULT_CONNECT_TIMEOUT = 10 * 1000;
		/**
     * 回复超时时间(默认30s)
     */
		public const int DEFAULT_RESPONSE_TIMEOUT = 30 * 1000;
		/**
     * 上传失败重试次数
     */
		public int RETRY_MAX = 5;

		/// <summary>
		/// The logger.
		/// </summary>
		public Util.ILogger Logger = null;

	}
}

