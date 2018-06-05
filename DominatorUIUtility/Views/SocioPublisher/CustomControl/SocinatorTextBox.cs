using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DominatorUIUtility.Views.SocioPublisher.CustomControl;assembly=DominatorUIUtility.Views.SocioPublisher.CustomControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:SocinatorTextBox/>
    ///
    /// </summary>
    [TemplatePart(Name = TextEditorPart, Type = typeof(TextBox))]
    [TemplatePart(Name = SuggestionPopUp, Type = typeof(Popup))]
    [TemplatePart(Name = SelectionListBox, Type = typeof(ListBox))]
    public class SocinatorTextBox : Control
    {
        static SocinatorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SocinatorTextBox), new FrameworkPropertyMetadata(typeof(SocinatorTextBox)));
        }

        #region Properties

        public const string TextEditorPart = "PART_Editor";
        public const string SuggestionPopUp = "PART_Popup";
        public const string SelectionListBox = "PART_Selector";

        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(SocinatorTextBox), new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SocinatorTextBox), new PropertyMetadata(string.Empty));

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDropDownOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SocinatorTextBox), new PropertyMetadata(false));


        public DataTemplate MacroTemplate
        {
            get { return (DataTemplate)GetValue(MacroTemplateProperty); }
            set { SetValue(MacroTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MacroTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MacroTemplateProperty =
            DependencyProperty.Register("MacroTemplate", typeof(DataTemplate), typeof(SocinatorTextBox), new PropertyMetadata(null));


        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(SocinatorTextBox));


        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(SocinatorTextBox), new PropertyMetadata(false));



        public IMacrosSuggestionProvider MacrosSuggestionProvider
        {
            get { return (IMacrosSuggestionProvider)GetValue(MacrosSuggestionProviderProperty); }
            set { SetValue(MacrosSuggestionProviderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MacrosSuggestionProvider.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MacrosSuggestionProviderProperty =
            DependencyProperty.Register("MacrosSuggestionProvider", typeof(IMacrosSuggestionProvider), typeof(SocinatorTextBox), new FrameworkPropertyMetadata(null));


        public object LoadingContent
        {
            get { return (object)GetValue(LoadingContentProperty); }
            set { SetValue(LoadingContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadingContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadingContentProperty =
            DependencyProperty.Register("LoadingContent", typeof(object), typeof(SocinatorTextBox), new PropertyMetadata(null));



        public string DisplayMember
        {
            get { return (string)GetValue(DisplayMemberProperty); }
            set { SetValue(DisplayMemberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMember.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMemberProperty =
            DependencyProperty.Register("DisplayMember", typeof(string), typeof(SocinatorTextBox), new PropertyMetadata(string.Empty));




        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SocinatorTextBox), new FrameworkPropertyMetadata(null, OnSelectedItemChanged));



        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Delay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(SocinatorTextBox), new PropertyMetadata(200));

        public BindingEvaluator BindingEvaluator { get; set; }

        public DispatcherTimer FetchTimer { get; set; }



        public string Filter { get; set; }

        public SelectionAdapter SelectionAdapter { get; set; }

        TextBox _postDescription = new TextBox();

        private Popup _postUp = new Popup();

        public Selector ItemsSelector { get; set; }

        private bool _isUpdatingText;

        private bool _selectionCancelled;

        private SuggestionsAdapter _suggestionsAdapter { get; set; }

        #endregion

        #region Apply Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));

            var postDescriptionText = Template.FindName(TextEditorPart, this) as TextBox;

            if (!_postDescription.Equals(postDescriptionText))
            {
                if (postDescriptionText != null)
                {
                    postDescriptionText.KeyDown -= OnEditorKeyDown;
                    postDescriptionText.LostFocus -= OnEditorLostFocus;
                    postDescriptionText.TextChanged -= OnEditorTextChanged;
                }

                _postDescription = postDescriptionText;

                if (postDescriptionText != null)
                {
                    postDescriptionText.KeyDown += OnEditorKeyDown;
                    postDescriptionText.LostFocus += OnEditorLostFocus;
                    postDescriptionText.TextChanged += OnEditorTextChanged;
                }
            }

            this.GotFocus += SocinatorTextBox_GotFocus;

            var popUpControl = Template.FindName(SuggestionPopUp, this) as Popup;

            if (!_postUp.Equals(popUpControl))
            {
                if (popUpControl != null)
                {
                    popUpControl.Opened -= OnPopupOpened;
                    popUpControl.Closed -= OnPopupClosed;
                }

                _postUp = popUpControl;

                if (popUpControl != null)
                {
                    popUpControl.StaysOpen = false;
                    popUpControl.Opened += OnPopupOpened;
                    popUpControl.Closed += OnPopupClosed;
                }
            }

            ItemsSelector = Template.FindName(SelectionListBox, this) as Selector;

            if (ItemsSelector != null)
            {
                SelectionAdapter = new SelectionAdapter(ItemsSelector);
                SelectionAdapter.Commit += OnSelectionAdapterCommit;
                SelectionAdapter.Cancel += OnSelectionAdapterCancel;
                SelectionAdapter.SelectionChanged += OnSelectionAdapterSelectionChanged;
                ItemsSelector.PreviewMouseDown += ItemsSelector_PreviewMouseDown;
            }
        }

        #endregion


        #region Events

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (!_selectionCancelled)
            {
                OnSelectionAdapterCommit();
            }
        }

        private void OnSelectionAdapterCommit()
        {
            if (ItemsSelector.SelectedItem != null)
            {
                SelectedItem = ItemsSelector.SelectedItem;
                _isUpdatingText = true;
                _postDescription.Text = GetDisplayText(ItemsSelector.SelectedItem);
                SetSelectedItem(ItemsSelector.SelectedItem);
                _isUpdatingText = false;
                IsDropDownOpen = false;
            }
        }

        private void OnSelectionAdapterCancel()
        {
            _isUpdatingText = true;
            _postDescription.Text = SelectedItem == null ? Filter : GetDisplayText(SelectedItem);
            _postDescription.SelectionStart = _postDescription.Text.Length;
            _postDescription.SelectionLength = 0;
            _isUpdatingText = false;
            IsDropDownOpen = false;
            _selectionCancelled = true;
        }

        private string GetDisplayText(object dataItem)
        {
            if (BindingEvaluator == null)
            {
                BindingEvaluator = new BindingEvaluator(new Binding(DisplayMember));
            }
            if (dataItem == null)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(DisplayMember))
            {
                return dataItem.ToString();
            }
            return BindingEvaluator.Evaluate(dataItem);
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            _selectionCancelled = false;
            ItemsSelector.SelectedItem = SelectedItem;
        }

        private void SocinatorTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _postDescription?.Focus();
        }

        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (SelectionAdapter != null)
            {
                if (IsDropDownOpen)
                    SelectionAdapter.HandleKeyDown(e);
                else
                    IsDropDownOpen = e.Key == Key.Down || e.Key == Key.Up;
            }
        }

        private void OnEditorLostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
            {
                IsDropDownOpen = false;
            }
        }

        private void OnEditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingText)
                return;
            if (FetchTimer == null)
            {
                FetchTimer = new DispatcherTimer();
                FetchTimer.Interval = TimeSpan.FromMilliseconds(Delay);
                FetchTimer.Tick += OnFetchTimerTick;
            }
            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            SetSelectedItem(null);
            if (_postDescription.Text.Length > 0)
            {
                IsLoading = true;
                IsDropDownOpen = true;
                ItemsSelector.ItemsSource = null;
                FetchTimer.IsEnabled = true;
                FetchTimer.Start();
            }
            else
            {
                IsDropDownOpen = false;
            }
        }

        private void SetSelectedItem(object item)
        {
            _isUpdatingText = true;
            SelectedItem = item;
            _isUpdatingText = false;
        }

        public static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SocinatorTextBox socinatorTextBox = null;
            socinatorTextBox = d as SocinatorTextBox;
            if (socinatorTextBox != null)
            {
                if (socinatorTextBox._postDescription != null & !socinatorTextBox._isUpdatingText)
                {
                    socinatorTextBox._isUpdatingText = true;
                    socinatorTextBox._postDescription.Text = socinatorTextBox.BindingEvaluator.Evaluate(e.NewValue);
                    socinatorTextBox._isUpdatingText = false;
                }
            }
        }

        private void OnFetchTimerTick(object sender, EventArgs e)
        {
            FetchTimer.IsEnabled = false;
            FetchTimer.Stop();
            if (MacrosSuggestionProvider != null && ItemsSelector != null)
            {
                Filter = _postDescription.Text;
                if (_suggestionsAdapter == null)
                {
                    _suggestionsAdapter = new SuggestionsAdapter(this);
                }
                _suggestionsAdapter.GetSuggestions(Filter);
            }
        }


        private void ItemsSelector_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos_item = (e.OriginalSource as FrameworkElement)?.DataContext;
            if (pos_item == null)
                return;
            if (!ItemsSelector.Items.Contains(pos_item))
                return;
            ItemsSelector.SelectedItem = pos_item;
            OnSelectionAdapterCommit();
        }

        private void OnSelectionAdapterSelectionChanged()
        {
            _isUpdatingText = true;
            _postDescription.Text = ItemsSelector.SelectedItem == null ? Filter : GetDisplayText(ItemsSelector.SelectedItem);
            _postDescription.SelectionStart = _postDescription.Text.Length;
            _postDescription.SelectionLength = 0;
            ScrollToSelectedItem();
            _isUpdatingText = false;
        }

        private void ScrollToSelectedItem()
        {
            ListBox listBox = ItemsSelector as ListBox;
            if (listBox != null && listBox.SelectedItem != null)
                listBox.ScrollIntoView(listBox.SelectedItem);
        }


        #endregion

        #region "Nested Types"

        private class SuggestionsAdapter
        {

            #region "Fields"

            private SocinatorTextBox _socinatorText;

            private string _filter;
            #endregion

            #region "Constructors"

            public SuggestionsAdapter(SocinatorTextBox socinatorText)
            {
                _socinatorText = socinatorText;
            }

            #endregion

            #region "Methods"

            public void GetSuggestions(string searchText)
            {
                _filter = searchText;
                _socinatorText.IsLoading = true;
                ParameterizedThreadStart thInfo = new ParameterizedThreadStart(GetSuggestionsAsync);
                Thread th = new Thread(thInfo);
                th.Start(new object[] {
                    searchText,
                    _socinatorText.MacrosSuggestionProvider
                });
            }

            private void DisplaySuggestions(IEnumerable suggestions, string filter)
            {
                if (_filter != filter)
                {
                    return;
                }
                if (_socinatorText.IsDropDownOpen)
                {
                    _socinatorText.IsLoading = false;
                    _socinatorText.ItemsSelector.ItemsSource = suggestions;
                    _socinatorText.IsDropDownOpen = _socinatorText.ItemsSelector.HasItems;
                }

            }

            private void GetSuggestionsAsync(object param)
            {
                object[] args = param as object[];
                string searchText = Convert.ToString(args[0]);
                IMacrosSuggestionProvider provider = args[1] as IMacrosSuggestionProvider;
                IEnumerable list = provider.GetMacrosSuggestions(searchText);
                _socinatorText.Dispatcher.BeginInvoke(new Action<IEnumerable, string>(DisplaySuggestions), DispatcherPriority.Background, new object[] {
                    list,
                    searchText
                });
            }

            #endregion

        }

        #endregion
    }


    public class MacrosTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SocinatorMacroModel)
                return TextTemplate;
            return base.SelectTemplate(item, container);
        }
    }

    public class MacrosSuggestionProvider : IMacrosSuggestionProvider
    {
        public IEnumerable<SocinatorMacroModel> ListOfMacros { get; set; }

        public IEnumerable GetMacrosSuggestions(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return null;

            if (!filter.StartsWith("{") && !filter.EndsWith("}"))
                return null;

            return ListOfMacros;
        }

        public MacrosSuggestionProvider()
        {
            SocinatorInitialize.Macros.Add(new SocinatorMacroModel { MacroKey = "{Hello}", MacroValue = "Hello" });
            SocinatorInitialize.Macros.Add(new SocinatorMacroModel { MacroKey = "{Globussoft}", MacroValue = "Globussoft" });
            ListOfMacros = SocinatorInitialize.Macros;
        }
    }

    public interface IMacrosSuggestionProvider
    {

        #region Public Methods

        IEnumerable GetMacrosSuggestions(string filter);

        #endregion Public Methods

    }

    public class BindingEvaluator : FrameworkElement
    {

        #region "Fields"


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(BindingEvaluator), new FrameworkPropertyMetadata(string.Empty));

        private Binding _valueBinding;
        #endregion

        #region "Constructors"

        public BindingEvaluator(Binding binding)
        {
            ValueBinding = binding;
        }

        #endregion

        #region "Properties"

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }

            set { SetValue(ValueProperty, value); }
        }

        public Binding ValueBinding
        {
            get { return _valueBinding; }
            set { _valueBinding = value; }
        }

        #endregion

        #region "Methods"

        public string Evaluate(object dataItem)
        {
            this.DataContext = dataItem;
            SetBinding(ValueProperty, ValueBinding);
            return Value;
        }

        #endregion

    }

    public class SelectionAdapter
    {

        #region "Fields"


        private Selector _selectorControl;
        #endregion

        #region "Constructors"

        public SelectionAdapter(Selector selector)
        {
            SelectorControl = selector;
            SelectorControl.PreviewMouseUp += OnSelectorMouseDown;
        }

        #endregion

        #region "Events"

        public delegate void CancelEventHandler();

        public delegate void CommitEventHandler();

        public delegate void SelectionChangedEventHandler();

        public event CancelEventHandler Cancel;
        public event CommitEventHandler Commit;
        public event SelectionChangedEventHandler SelectionChanged;
        #endregion

        #region "Properties"

        public Selector SelectorControl
        {
            get { return _selectorControl; }
            set { _selectorControl = value; }
        }

        #endregion

        #region "Methods"

        public void HandleKeyDown(KeyEventArgs key)
        {
            Debug.WriteLine(key.Key);
            switch (key.Key)
            {
                case Key.Down:
                    IncrementSelection();
                    break;
                case Key.Up:
                    DecrementSelection();
                    break;
                case Key.Enter:
                    if (Commit != null)
                    {
                        Commit();
                    }

                    break;
                case Key.Escape:
                    if (Cancel != null)
                    {
                        Cancel();
                    }

                    break;
                case Key.Tab:
                    if (Commit != null)
                    {
                        Commit();
                    }

                    break;
            }
        }

        private void DecrementSelection()
        {
            if (SelectorControl.SelectedIndex == -1)
            {
                SelectorControl.SelectedIndex = SelectorControl.Items.Count - 1;
            }
            else
            {
                SelectorControl.SelectedIndex -= 1;
            }
            if (SelectionChanged != null)
            {
                SelectionChanged();
            }
        }

        private void IncrementSelection()
        {
            if (SelectorControl.SelectedIndex == SelectorControl.Items.Count - 1)
            {
                SelectorControl.SelectedIndex = -1;
            }
            else
            {
                SelectorControl.SelectedIndex += 1;
            }
            if (SelectionChanged != null)
            {
                SelectionChanged();
            }
        }

        private void OnSelectorMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Commit != null)
            {
                Commit();
            }
        }

        #endregion

    }

}
