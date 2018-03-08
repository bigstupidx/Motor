using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;
using XPlugin.Data.Json;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;

public class IconUtil {

	public static Dictionary<int, string> IconList = new Dictionary<int, string>() {
		{36,     "drawable-ldpi"},
		{48,     "drawable-mdpi"},
		{72,     "drawable-hdpi"},
		{96,     "drawable-xhdpi"},
		{144,    "drawable-xxhdpi"},
		{192,    "drawable-xxxhdpi"}
	};


	public static void ApplyAndroidIcon(string projPath) {
		var configPath = Path.Combine(projPath, "icon/icon.json");
		float connerPercent;
		string connerName;
		if (File.Exists(configPath)) {
			//读取配置文件
			string content = File.ReadAllText(configPath);
			JObject configRoot = JObject.Parse(content);
			connerPercent = configRoot["round"].OptFloat();
			connerName = configRoot["conner"].OptString("");
		} else {
			connerPercent = 0;
			connerName = "";
		}

		var cornerFilePath = Path.Combine(projPath, "icon/" + connerName);//角标
		if (string.IsNullOrEmpty(connerName) || !File.Exists(cornerFilePath)) {
			cornerFilePath = null;
		}

		var iconFilePath = Path.Combine(projPath, "icon/icon.png");
		if (!File.Exists(iconFilePath)) {
			if (cornerFilePath == null && connerPercent == 0) {//既没有角标也没有图标也没有圆角，不需要处理
				return;
			}
			//没有图标有角标或者有圆角的，使用xxxhdpi中的图标作为图标
			var iconPath = Path.Combine(projPath, "res/drawable-xxxhdpi/app_icon.png");
			File.Copy(iconPath, iconFilePath);
		}

		//切圆角并保存
		if (connerPercent != 0) {
			var genrateIconPath = Path.Combine(projPath, "icon/generate-round.png");
			RoundCorners(Image.FromFile(iconFilePath), connerPercent, Color.Transparent).Save(genrateIconPath);
			iconFilePath = genrateIconPath;
		}

		//叠加角标，并保存
		if (cornerFilePath != null) {
			var genrateIconPath = Path.Combine(projPath, "icon/generate-overlay.png");
			Overlay(iconFilePath, cornerFilePath, genrateIconPath);
			iconFilePath = genrateIconPath;
		}

		foreach (var iconSizeInfo in IconList) {
			var targetPath = Path.Combine(projPath, "res/" + iconSizeInfo.Value + "/app_icon.png");
			var size = iconSizeInfo.Key;
			ResizeIcon(iconFilePath, targetPath, size, size);
		}
	}

	public static void Overlay(string imageFile, string overlayImgFile, string saveFile) {
		using (var overlayImage = Image.FromFile(overlayImgFile))
		using (var srcImage = Image.FromFile(imageFile)) {
			using (var newImage = new Bitmap(srcImage.Width, srcImage.Height))
			using (var graphics = Graphics.FromImage(newImage)) {
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				var rect = new Rectangle(0, 0, srcImage.Width, srcImage.Height);
				graphics.DrawImage(srcImage, rect);
				graphics.DrawImage(overlayImage, rect);
				newImage.Save(saveFile);
			}
		}
	}

	public static void ResizeIcon(string imageFile, string outputFile, int width, int height) {
		using (var srcImage = Image.FromFile(imageFile)) {
			using (var newImage = new Bitmap(width, height))
			using (var graphics = Graphics.FromImage(newImage)) {
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
				newImage.Save(outputFile);
			}
		}
	}

	public static void ResizeIconWithOverlay(string imageFile, string overlayImgFile, string outputFile, int width, int height) {
		using (var overlayImage = Image.FromFile(overlayImgFile))
		using (var srcImage = Image.FromFile(imageFile)) {
			using (var newImage = new Bitmap(width, height))
			using (var graphics = Graphics.FromImage(newImage)) {
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				var rect = new Rectangle(0, 0, width, height);
				graphics.DrawImage(srcImage, rect);
				graphics.DrawImage(overlayImage, rect);
				newImage.Save(outputFile);
			}
		}
	}

	public static Image RoundCorners(Image StartImage, float CornerPercent, Color BackgroundColor) {
		int CornerRadius = (int)(StartImage.Width * CornerPercent);
		Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
		using (Graphics g = Graphics.FromImage(RoundedImage)) {
			g.Clear(BackgroundColor);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Brush brush = new TextureBrush(StartImage);
			GraphicsPath gp = new GraphicsPath();
			gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
			gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
			gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
			gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
			g.FillPath(brush, gp);
			return RoundedImage;
		}
	}
}
