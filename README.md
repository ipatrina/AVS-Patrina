# AVS Patrina

AVS Patrina 是一款适用于将MPEG-2传输流中采用AVS1-P16广播视频(AVS+)编码标准的图像转换为MPEG-4 Part 10 (H.264)编码标准，并保留MPEG-2传输流中原始音频数据的非实时视频编码转换器。

AVS Patrina 2.0 采用全球首屈一指的MainConcept® AVC(软件)编码器，为您带来广播级H.264编码的AVS+转码体验。


![AVS Patrina preview](https://thumbs2.imgbox.com/7b/c4/Z6UrxGyf_t.png)


# 安装和使用

本软件适用于Windows 7及以上操作系统。

若要开始使用AVS Patrina 2.0进行工作，您需要准备以下组件。请注意，这些组件可能涉及商业授权或商业机密。

- 包含AVS1-P16(AVS+)支持的GDM解码演示可执行程序

- MainConcept® SDK 开发套件

- FFmpeg 软件

---

您需要将各组件(包)对应的文件与AVS Patrina主程序放置于同一目录。

**GDM**

- ldecod.exe

您可以向AVS工作组申请GDM演示包，并使用Visual Studio编译源代码，生成所需可执行文件。

**MainConcept® SDK**

- demo_config_avc.dll

- demo_enc_avc.dll

- demo_mux_mp2.dll

- sample_enc_avc.exe

- sample_enc_avc.ini

- sample_mux_mp2_file.exe

若您持有MainConcept® AVC编码器商业授权，请将已授权的DLL文件重命名为“demo_enc_avc.dll”并进行放置。

通过编辑“sample_enc_avc.ini”文件可实现理想的H.264编码参数。

您可以通过MainConcept官网申请试用MainConcept® SDK产品：https://www.mainconcept.com/avc-demo

**FFmpeg**

- ffmpeg.exe

建议您使用 FFmpeg 5.0.1 版本。示例下载：https://github.com/marierose147/ffmpeg_windows_exe_with_fdk_aac/releases/tag/%2335

使用其他版本的FFmpeg软件可能会导致文件读取异常。AVS Patrina 2.0仅使用FFmpeg软件对输入文件进行基本信息获取。FFmpeg软件不会被用于编解码及封装过程。

