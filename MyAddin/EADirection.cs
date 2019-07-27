using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAddin
{
    class EADirection
    {
        private int left = 50;
        private int right = 100;
        private int top = 100;
        private int bottom = 200;

        const int eachRowNum = 3;

        const int eachWidth = 150;

    //    int eachHeight = 400;

        private int start = 0;

        public EADirection() { }

        public EADirection(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public int Left
        {
            get { return left; }
            set { left = value; }
        }

        public int Right
        {
            get { return right; }
            set { right = value; }
        }

        public int Top
        {
            get { return top; }
            set { top = value; }
        }

        public int Bottom
        {
            get { return bottom; }
            set { bottom = value; }
        }

        public int Start
        {
            get { return start; }
            set { start = value; }
        }

        public EADirection nextEADirection()
        {
            int row = start / eachRowNum;
            int col = start % eachRowNum;
            this.start++;
            return new EADirection(left + col * eachWidth, this.right + col * eachWidth, top + eachWidth * row, bottom);
        }

        public void reset()
        {
            this.start = 0;
        }

        override
        public String ToString()
        {
            return String.Format("l={0};r={1};t={2};b={3};", left, right, top, bottom);
        }
    }
}
