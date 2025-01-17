﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Shapoco.Calctus.Model;

namespace Shapoco.Calctus.UI {
    internal class HistoryBox : ListBox {
        public HistoryBox() {
            if (this.DesignMode) return;
            this.DoubleBuffered = true;
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.DrawItem += HistoryBox_DrawItem;
            this.MeasureItem += HistoryBox_MeasureItem;
        }

        /// <summary>アイテムの表示サイズ計算</summary>
        private void HistoryBox_MeasureItem(object sender, MeasureItemEventArgs e) {
            if (this.DesignMode) return;
            if (e.Index < 0) return;
            var g = e.Graphics;
            var item = (HistoryItem)Items[e.Index];
            var size = g.MeasureString(item.ToString(), this.Font);
            e.ItemWidth = (int)Math.Ceiling(size.Width);
            e.ItemHeight = (int)Math.Ceiling(size.Height);
        }

        /// <summary>アイテムの描画</summary>
        private void HistoryBox_DrawItem(object sender, DrawItemEventArgs e) {
            if (this.DesignMode) return;
            if (e.Index < 0) return;
            var g = e.Graphics;

            var item = (HistoryItem)Items[e.Index];
            var backColor = this.BackColor;
            var textColor = this.ForeColor;
            if (SelectedIndex == e.Index) {
                backColor = this.Focused ? SystemColors.Highlight : ColorUtils.Grayish(backColor, 20);
                textColor = this.Focused ? SystemColors.HighlightText : this.ForeColor;
            }
            if (!item.IsEmpty && item.Error) {
                textColor = Color.Red;
            }
            using (var brush = new SolidBrush(backColor)) {
                g.FillRectangle(brush, e.Bounds);
            }
            using (var brush = new SolidBrush(textColor)) {
                g.DrawString(item.ToString(), Font, brush, e.Bounds.Location);
            }
        }
    }
}
