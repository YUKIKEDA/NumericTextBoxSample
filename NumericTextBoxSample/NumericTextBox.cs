﻿using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NumericTextBoxSample
{
    public partial class NumericTextBox : TextBox
    {
        [GeneratedRegex("[^0-9.-]+", RegexOptions.Compiled)]
        private static partial Regex NumericRegex();

        [GeneratedRegex("[^0-9-]+", RegexOptions.Compiled)]
        private static partial Regex IntegerRegex();

        private static readonly Regex _numericRegex = NumericRegex();
        private static readonly Regex _integerRegex = IntegerRegex();

        public NumericTextBox()
        {
            // 入力時の検証を有効化
            this.PreviewTextInput += NumericTextBox_PreviewTextInput;

            // コピー＆ペーストの制御
            DataObject.AddPastingHandler(this, OnPaste);
        }

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 入力された文字が数値かどうかチェック
            e.Handled = IsTextNumeric(e.Text);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (IsTextNumeric(text))
                {
                    e.CancelCommand();
                }
            }
        }

        private bool IsTextNumeric(string text)
        {
            return IntegerOnly ? _integerRegex.IsMatch(text) : _numericRegex.IsMatch(text);
        }

        // 数値の最小値プロパティ
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MinValue));

        // 数値の最大値プロパティ
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumericTextBox), new PropertyMetadata(double.MaxValue));

        // バリデーションメッセージ用のプロパティ
        public string ValidationMessage
        {
            get { return (string)GetValue(ValidationMessageProperty); }
            set { SetValue(ValidationMessageProperty, value); }
        }

        public static readonly DependencyProperty ValidationMessageProperty =
            DependencyProperty.Register("ValidationMessage", typeof(string), typeof(NumericTextBox), new PropertyMetadata(string.Empty));

        // 最小値を含むかどうかのプロパティ
        public bool IncludeMinimum
        {
            get { return (bool)GetValue(IncludeMinimumProperty); }
            set { SetValue(IncludeMinimumProperty, value); }
        }

        public static readonly DependencyProperty IncludeMinimumProperty =
            DependencyProperty.Register("IncludeMinimum", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(true));

        // 最大値を含むかどうかのプロパティ
        public bool IncludeMaximum
        {
            get { return (bool)GetValue(IncludeMaximumProperty); }
            set { SetValue(IncludeMaximumProperty, value); }
        }

        public static readonly DependencyProperty IncludeMaximumProperty =
            DependencyProperty.Register("IncludeMaximum", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(true));

        // 最小値バリデーション用のカスタムメッセージ
        public string MinimumValidationMessage
        {
            get { return (string)GetValue(MinimumValidationMessageProperty); }
            set { SetValue(MinimumValidationMessageProperty, value); }
        }

        public static readonly DependencyProperty MinimumValidationMessageProperty =
            DependencyProperty.Register("MinimumValidationMessage", typeof(string), typeof(NumericTextBox), 
                new PropertyMetadata(string.Empty));

        // 最大値バリデーション用のカスタムメッセージ
        public string MaximumValidationMessage
        {
            get { return (string)GetValue(MaximumValidationMessageProperty); }
            set { SetValue(MaximumValidationMessageProperty, value); }
        }

        public static readonly DependencyProperty MaximumValidationMessageProperty =
            DependencyProperty.Register("MaximumValidationMessage", typeof(string), typeof(NumericTextBox), 
                new PropertyMetadata(string.Empty));

        // 数値形式バリデーション用のカスタムメッセージ
        public string FormatValidationMessage
        {
            get { return (string)GetValue(FormatValidationMessageProperty); }
            set { SetValue(FormatValidationMessageProperty, value); }
        }

        public static readonly DependencyProperty FormatValidationMessageProperty =
            DependencyProperty.Register("FormatValidationMessage", typeof(string), typeof(NumericTextBox), 
                new PropertyMetadata(string.Empty));

        // 整数入力限定プロパティ
        public bool IntegerOnly
        {
            get { return (bool)GetValue(IntegerOnlyProperty); }
            set { SetValue(IntegerOnlyProperty, value); }
        }

        public static readonly DependencyProperty IntegerOnlyProperty =
            DependencyProperty.Register("IntegerOnly", typeof(bool), typeof(NumericTextBox), new PropertyMetadata(false));

        // 値の検証
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (string.IsNullOrEmpty(Text))
            {
                ValidationMessage = string.Empty;
                return;
            }

            bool isValid;
            double doubleValue;
            
            if (IntegerOnly)
            {
                isValid = int.TryParse(Text, out int intValue);
                doubleValue = intValue;
            }
            else
            {
                isValid = double.TryParse(Text, out doubleValue);
            }

            if (isValid)
            {
                if ((IncludeMinimum && doubleValue < Minimum) || (!IncludeMinimum && doubleValue <= Minimum))
                {
                    ValidationMessage = !string.IsNullOrEmpty(MinimumValidationMessage)
                        ? MinimumValidationMessage
                        : IncludeMinimum
                            ? $"Value must be greater than or equal to {Minimum}."
                            : $"Value must be greater than {Minimum}.";
                }
                else if ((IncludeMaximum && doubleValue > Maximum) || (!IncludeMaximum && doubleValue >= Maximum))
                {
                    ValidationMessage = !string.IsNullOrEmpty(MaximumValidationMessage)
                        ? MaximumValidationMessage
                        : IncludeMaximum
                            ? $"Value must be less than or equal to {Maximum}."
                            : $"Value must be less than {Maximum}.";
                }
                else
                {
                    ValidationMessage = string.Empty;
                }
            }
            else
            {
                ValidationMessage = !string.IsNullOrEmpty(FormatValidationMessage)
                    ? FormatValidationMessage
                    : IntegerOnly ? "Please enter a valid integer." : "Please enter a valid number.";
            }
        }
    }
}
