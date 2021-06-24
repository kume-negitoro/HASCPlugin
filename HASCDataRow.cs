using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using Plugin;

namespace HASCPlugin {
    public sealed class HASCDataRow : DataRow {
        private IPluginHost host;

        private readonly List<double> timestampList = default;
        private readonly List<double> valueList = default;
        private double minValue;
        private double maxValue;

        public HASCDataRow(
            IPluginHost host,
            string name,
            List<double> timestampList,
            List<double> valueList
        ) : base(host)
        {
            this.host = host;
            this.Title = name;
            this.timestampList = timestampList;
            this.valueList = valueList;

            CalcMinMaxValue();

            this.RowHeight *= 6;

            this.RowPanel.Paint += new PaintEventHandler(HandlePaint);
        }

        private void CalcMinMaxValue()
        {
            var min = Double.MaxValue;
            var max = Double.MinValue;
            foreach(var value in valueList)
            {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
            this.minValue = min;
            this.maxValue = max;
        }

        private void HandlePaint(object sender, PaintEventArgs e)
        {
            using (Bitmap bmp = new Bitmap(((Control)sender).Width, ((Control)sender).Height))
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen pen = new Pen(Color.Black, 1))
            {
                // MessageBox.Show("width: " + ((Control)sender).Width + ", height: " + ((Control)sender).Height);
                try
                {
                    //表示領域のラインの点を算出する
                    Point[] points = GetDrawPoints().ToArray();
                    g.DrawLines(pen, points);
                }
                catch { }
                finally
                {
                    //画面に反映
                    e.Graphics.DrawImage(bmp, 0, 0);
                }
            }
        }

        private List<Point> GetDrawPoints()
        {
            var points = new List<Point>();

            double elapsed = 0;
            int pixel = 0;
            
            if(elapsed >= host.FieldStartTime)
            {
                points.Add(new Point(
                    pixel++,
                    (int)HASCDataRow.LinearMap(valueList[0], -1.5, 1.5, this.RowHeight, 0)
                ));
            }
            var i = 1;
            for(; i < valueList.Count; i++)
            {
                elapsed += timestampList[i] - timestampList[i-1];

                // // ゴミのような実装
                if(elapsed <= host.FieldStartTime) continue;
                if(elapsed > host.FieldEndTime) break;

                // 右辺は横軸のピクセル数からプロット毎の経過時間を算出する(例えばppsが60であれば2ピクセル目は2/60秒)
                // delta(CSVデータの時間で見て、合計の経過時間を表す)がプロット毎の経過時間を超えていたらプロットする
                if(elapsed >= (pixel * (1 / host.PixelPerSec)))
                {
                    var index = (int)HASCDataRow.LinearMap(elapsed - host.FieldStartTime, host.FieldStartTime, host.FieldEndTime, 0, this.RowPanel.Width);
                    points.Add(new Point(
                        pixel++,
                        // (int)(valueList[i] * 100)
                        (int)HASCDataRow.LinearMap(valueList[i], -1.5, 1.5, this.RowHeight, 0)
                    ));
                }
            }
            // MessageBox.Show("i: " + i);

            // MessageBox.Show("ExpectedWidth: " + this.RowPanel.Width + ", RealWidth: " + pixel * host.PixelPerSec);

            return points;
        }

        public static double LinearMap(double value, double start1, double end1, double start2, double end2)
        {
            return start2 + (end2 - start2) * ((value - start1) / (end1 - start1));
        }
    }
}