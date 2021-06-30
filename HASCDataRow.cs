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

        private double CommonOffset {
            get
            {
                return timestampList[0];
            }
        }

        private IEnumerable<double> TimestampWithoutCommonOffset {
            get
            {
                foreach(var timestamp in timestampList)
                {
                    yield return timestamp - CommonOffset;
                }
            }
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

            var i = 0; // Linqを使いたかった
            foreach(var timestamp in TimestampWithoutCommonOffset)
            {
                // 表示範囲の開始時間までスキップ
                if(timestamp > host.FieldStartTime)
                {
                    points.Add(
                        new Point(
                            (int)((timestamp - host.FieldStartTime) * host.PixelPerSec),
                            (int)Utils.LinearMap(valueList[i], -1.5, 1.5, this.RowHeight, 0)
                        )
                    );
                }
                i++;

                // 表示範囲を超えたら修了
                if(timestamp > host.FieldEndTime) break;
            }

            return points;
        }
    }
}