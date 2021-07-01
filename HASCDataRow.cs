using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.VisualBasic;

using Plugin;

namespace HASCPlugin {

    public class ChangeOffsetArgs : EventArgs
    {
        public double Offset
        {
            get; set;
        }

        public ChangeOffsetArgs(double offset)
        {
            Offset = offset;
        }
    }

    public sealed class HASCDataRow : DataRow {
        private IPluginHost host;
        
        public event EventHandler<ChangeOffsetArgs> ChangeOffset;

        private double offset = 0;

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

            // コンテキストメニューの追加
            var separator = new ToolStripSeparator();
            var offsetItem = new ToolStripMenuItem();
            offsetItem.Text = "オフセットを設定";
            offsetItem.Name = "offsetItem";
            offsetItem.Click += new EventHandler(HandleOffsetItemClick);
            ContextMenuItems.Add(separator);
            ContextMenuItems.Add(offsetItem);

            this.RowHeight *= 6;

            this.RowPanel.Paint += new PaintEventHandler(HandlePaint);
        }

        public double Offset {
            get
            {
                return offset;
            }
            set
            {
                if(offset == value) return;
                offset = value;
                ChangeOffset(this, new ChangeOffsetArgs(offset));
                RowPanel.Refresh();
            }
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

        private void HandleOffsetItemClick(object sender, EventArgs e)
        {
            var offsetString = Interaction.InputBox(
                "オフセットを実数値で入力してください (秒)",
                "同期オフセットの設定",
                "" + Offset,
                -1,
                -1
            );

            if(offsetString == null) return;

            double offsetTime;
            try
            {
                offsetTime = Double.Parse(offsetString);
            }
            catch
            {
                MessageBox.Show("オフセット値が不正です");
                return;
            }
            
            Offset = offsetTime;
            ChangeOffset(this, new ChangeOffsetArgs(offset));
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
                if(timestamp + Offset > host.FieldStartTime)
                {
                    points.Add(
                        new Point(
                            (int)((timestamp + Offset - host.FieldStartTime) * host.PixelPerSec),
                            (int)Utils.LinearMap(valueList[i], -1.5, 1.5, this.RowHeight, 0)
                        )
                    );
                }
                i++;

                // 表示範囲を超えたら修了
                if(timestamp + Offset > host.FieldEndTime) break;
            }

            return points;
        }
    }
}