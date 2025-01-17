﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shapoco.Calctus.Model.Syntax;
using Shapoco.Calctus.Model.UnitSystem;

namespace Shapoco.Calctus.Model {
    class RealVal : Val {
        private Unit _unit;
        public override Unit Unit => _unit;

        private double _raw;
        public RealVal(double val, ValFormatHint fmt = null, Unit unit = null) : base(fmt) {
            this._raw = val;
            this._unit = unit != null ? unit : NativeUnits.Dimless;
        }

        public override object Raw => _raw;

        public override bool IsScalar => true;
        public override bool IsInteger => (_raw == (long)_raw);

        protected override Val OnAsDimless(EvalContext e) 
            => new RealVal(_raw, this.FormatHint);

        protected override Val OnUpConvert(EvalContext e, Val b) {
            if (b is RealVal) return this;
            throw new InvalidCastException(this.ValTypeName + " cannot be converted to " + b.ValTypeName);
        }

        protected override Val OnUnaryPlus(EvalContext e) => this;
        protected override Val OnAtirhInv(EvalContext e) => new RealVal(-_raw, FormatHint, Unit);
        protected override Val OnBitNot(EvalContext e) => new RealVal(~this.AsInt, FormatHint, Unit);

        protected override Val OnAdd(EvalContext e, Val b) {
            this.Unit.AssertDimensionEquality( b.Unit, UnitEnumMode.Dimension);
            return new RealVal(_raw + b.AsDouble, FormatHint, Unit);
        }
        protected override Val OnSub(EvalContext e, Val b) {
            this.Unit.AssertDimensionEquality(b.Unit, UnitEnumMode.Dimension);
            return new RealVal(_raw - b.AsDouble, FormatHint, Unit);
        }
        
        protected override Val OnMul(EvalContext e, Val b) {
            var bVal = b.AsRealVal();
            var val = _raw * bVal._raw;
            var unit = this.Unit.Mul(e, bVal.Unit);
            return new RealVal(val, FormatHint, unit);
        }
        
        protected override Val OnDiv(EvalContext e, Val b) {
            var bVal = b.AsRealVal();
            var val = _raw / bVal._raw;
            var unit = this.Unit.Mul(e, bVal.Unit.Invert(e));
            return new RealVal(val, FormatHint, unit);
        }

        protected override Val OnIDiv(EvalContext e, Val b) {
            var bVal = b.AsRealVal();
            var val = Math.Truncate(_raw / bVal._raw);
            var unit = this.Unit.Mul(e, bVal.Unit.Invert(e));
            return new RealVal(val, FormatHint, unit);
        }

        protected override Val OnMod(EvalContext e, Val b) {
            return new RealVal(_raw % b.AsDouble, FormatHint, this.Unit);
        }

        // todo: 単位の考慮
        protected override Val OnLogicShiftL(EvalContext e, Val b) => new RealVal(this.AsInt << b.AsInt, FormatHint);
        protected override Val OnLogicShiftR(EvalContext e, Val b) => new RealVal((int)((uint)this.AsInt >> b.AsInt), FormatHint);
        protected override Val OnArithShiftL(EvalContext e, Val b) {
            var a = this.AsInt;
            var sign = a & (1 << 31);
            var lshift = (a << b.AsInt) & 0x7fffffff;
            return new RealVal(sign | lshift, FormatHint);
        }
        protected override Val OnArithShiftR(EvalContext e, Val b) => new RealVal(this.AsInt >> b.AsInt, FormatHint);

        // todo: 単位の考慮
        protected override Val OnBitAnd(EvalContext e, Val b) => new RealVal(this.AsInt & b.AsInt, FormatHint);
        protected override Val OnBitXor(EvalContext e, Val b) => new RealVal(this.AsInt ^ b.AsInt, FormatHint);
        protected override Val OnBitOr(EvalContext e, Val b) => new RealVal(this.AsInt | b.AsInt, FormatHint);

        protected override Val OnFormat(ValFormatHint fmt) => new RealVal(_raw, fmt, Unit);

        protected override RealVal OnAsRealVal() => new RealVal((double)Raw, FormatHint, Unit);
        public override double AsDouble => _raw;
        public override long AsLong => (long)_raw; // todo: 丸め/切り捨ての明示は不要？
        public override int AsInt => (int)_raw; // todo: 丸め/切り捨ての明示は不要？

        public override string ToString() {
            return ToString(new EvalContext());
        }

        public string ToString(EvalContext e) {
            if (Unit.IsDimless) {
                return FormatHint.Formatter.Format(this);
            }
            else {
                return Unit.ScaleValue(e, this.AsDouble).ToString() + "[" + Unit.ToString() + "]";
            }
        }
        //public static implicit operator double(RealVal val) => val.AsDouble();
        //public static implicit operator RealVal(double val) => new RealVal(val);

        //public static explicit operator long(RealVal val) => val.AsLong();
        //public static explicit operator RealVal(long val) => new RealVal(val);
    }
}
