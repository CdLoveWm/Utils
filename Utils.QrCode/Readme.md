> **二维码生成**

引用Nuget包`SkiaSharp.QrCode`

```c#
1、var info = new SKImageInfo(512, 512); // 用于指定生成的二维码图片的大小

2、using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
   // SKEncodedImageFormat.Png 可以指定生成的二维码图片的格式。
```



