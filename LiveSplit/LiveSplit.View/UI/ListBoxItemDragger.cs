﻿using LiveSplit.Model;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LiveSplit.UI
{

    /// <summary>
    /// Turn on item dragging for some ListBox control
    /// </summary>
    public class ListBoxItemDragger
    {
        private ListBox listBox;
        public Form Form { get; set; }

        //public ReaderWriterLockSlim DrawLock { get; set; }

        private int dragItemIndex = -1;

        /// <summary>
        /// Gets the index of the dragged item.
        /// </summary>
        /// <value>The index of the dragged item.</value>
        public int DragItemIndex
        {
            get { return dragItemIndex; }
        }

        private bool dragging = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ListBoxItemDragger"> class.
        /// </see></summary>
        /// <param name="listBox">The list box.
        public ListBoxItemDragger(ListBox listBox, Form form)
        {
            Attach(listBox);
            Form = form;
        }

        /// <summary>
        /// Attaches current instance to some ListBox control.
        /// </summary>
        public void Attach(ListBox listBox)
        {
            this.listBox = listBox;
            this.listBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDownHandler);
            this.listBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpHandler);
            this.listBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveHandler);
        }

        /// <summary>
        /// Detaches current instance from ListBox control.
        /// </summary>
        public void Detach()
        {
            this.listBox.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.MouseDownHandler);
            this.listBox.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.MouseUpHandler);
            this.listBox.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.MouseMoveHandler);
        }

        private Cursor dragCursor = Cursors.SizeNS;
        public Cursor DragCursor
        {
            get { return dragCursor; }
            set { dragCursor = value; }
        }

        /// <summary>
        /// Raises the <see cref="E:ItemMoved"> event.
        /// </see></summary>
        /// <param name="e">The <see cref="T:System.EventArgs"> instance containing the event data.
        protected void OnItemMoved(EventArgs e)
        {
            if (ItemMoved != null) ItemMoved(this, e);
        }

        /// <summary>
        /// Occurs when some item has been moved
        /// </summary>
        public event EventHandler ItemMoved;

        private void MouseDownHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            dragItemIndex = listBox.SelectedIndex;
        }


        private Cursor prevCursor = Cursors.Default;

        private void MouseUpHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            dragItemIndex = -1;
            if (dragging)
            {
                listBox.Cursor = prevCursor;
                dragging = false;
            }
        }

        private void MouseMoveHandler(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //if (!DrawLock.TryEnterWriteLock(500))
            //return;
            Action x = () =>
            {
                try
                {
                    if (dragItemIndex >= 0 && e.Y > 0)
                    {
                        if (!dragging)
                        {
                            dragging = true;
                            prevCursor = listBox.Cursor;
                            listBox.Cursor = DragCursor;
                        }
                        int dstIndex = listBox.IndexFromPoint(e.X, e.Y);

                        if (dragItemIndex != dstIndex)
                        {
                            dynamic item = listBox.Items[dragItemIndex];
                            listBox.BeginUpdate();
                            try
                            {
                                dynamic bindingList = listBox.DataSource;

                                bindingList.RemoveAt(dragItemIndex);
                                if (dstIndex != ListBox.NoMatches)
                                    bindingList.Insert(dstIndex, item);
                                else
                                {
                                    bindingList.Add(item);
                                    dstIndex = bindingList.Count - 1;
                                }

                                listBox.SelectedIndex = dstIndex;
                            }
                            finally
                            {
                                listBox.EndUpdate();
                            }
                            dragItemIndex = dstIndex;
                            OnItemMoved(EventArgs.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            };

            if (Form.InvokeRequired)
                Form.Invoke(x);
            else
                x();
            //DrawLock.ExitWriteLock();
        }

    }
}