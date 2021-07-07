using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static StackPanel _curEditor;
        private static DataGrid _curDataGrid;
        public MainWindow()
        {
            InitializeComponent();
            DtGridEmp.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
            DtGridDep.ItemsSource = TestTaskEntities.GetContext().Departments.ToList();
            DtGridOrd.ItemsSource = TestTaskEntities.GetContext().Orders.ToList();
            DepList.ItemsSource = TestTaskEntities.GetContext().Departments.ToList();
            EmpList1.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
            EmpList2.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
            EmpButton.Style = (Style)FindResource("PressedButton");
            _curEditor = EmpEditor;
            _curDataGrid = DtGridEmp;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void MinButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void EmpButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDataGrid(DtGridEmp, EmpButton);
        }

        private void DepButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDataGrid(DtGridDep, DepButton);
        }

        private void OrdButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDataGrid(DtGridOrd, OrdButton);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            HideEditor();
            DataContext = (sender as Button).DataContext;
            ShowEditor("Редактор элемента");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            HideEditor();

            if (_curDataGrid == DtGridEmp)
                DataContext = new Employees();
            else if (_curDataGrid == DtGridDep)
                DataContext = new Departments();
            else
                DataContext = new Orders();

            ShowEditor("Добавление элемента");
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_curDataGrid == DtGridEmp)
            {
                var selectedItems = _curDataGrid.SelectedItems.Cast<Employees>().ToList();
                TestTaskEntities.GetContext().Employees.RemoveRange(selectedItems);
                TestTaskEntities.GetContext().SaveChanges();
                _curDataGrid.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
                DtGridDep.ItemsSource = TestTaskEntities.GetContext().Departments.ToList();
                DtGridOrd.ItemsSource = TestTaskEntities.GetContext().Orders.ToList();
                EmpList1.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
                EmpList2.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
            }
            else if (_curDataGrid == DtGridDep)
            {
                var selectedItems = _curDataGrid.SelectedItems.Cast<Departments>().ToList();
                TestTaskEntities.GetContext().Departments.RemoveRange(selectedItems);
                TestTaskEntities.GetContext().SaveChanges();
                _curDataGrid.ItemsSource = TestTaskEntities.GetContext().Departments.ToList();
                DtGridEmp.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
                DepList.ItemsSource = TestTaskEntities.GetContext().Departments.ToList();
            }
            else
            {
                var selectedItems = _curDataGrid.SelectedItems.Cast<Orders>().ToList();
                TestTaskEntities.GetContext().Orders.RemoveRange(selectedItems);
                TestTaskEntities.GetContext().SaveChanges();
                _curDataGrid.ItemsSource = TestTaskEntities.GetContext().Orders.ToList();
            }

            HideEditor();
        }

        /// <summary>
        /// Скрывает панель редактирования, соответствующую свойству "DataContext".
        /// </summary>
        private void HideEditor()
        {
            DataContext = null;
            ActionType.Text = String.Empty;
            SaveButton.Visibility = Visibility.Hidden;
            ClearButton.Visibility = Visibility.Hidden;
            _curEditor.Visibility = Visibility.Hidden;
            VodovozImg.Opacity = 1;

            if (_curDataGrid == DtGridEmp)
                _curEditor = EmpEditor;
            else if (_curDataGrid == DtGridDep)
                _curEditor = DepEditor;
            else
                _curEditor = OrdEditor;
        }

        /// <summary>
        /// Выводит панель редактирования, соответствующую свойству "DataContext".
        /// </summary>
        private void ShowEditor(string actionType)
        {
            ActionType.Text = actionType;
            VodovozImg.Opacity = 0.12;
            SaveButton.Visibility = Visibility.Visible;
            ClearButton.Visibility = Visibility.Visible;
            _curEditor.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Отображает элемент DataGrid, переданный в качестве параметра.
        /// </summary>
        private void ShowDataGrid(DataGrid grid, Button button)
        {
            EmpButton.Style = (Style)FindResource("DefaultButton");
            DepButton.Style = (Style)FindResource("DefaultButton");
            OrdButton.Style = (Style)FindResource("DefaultButton");
            button.Style = (Style)FindResource("PressedButton");
            _curDataGrid.Visibility = Visibility.Hidden;
            _curDataGrid = grid;
            _curDataGrid.Visibility = Visibility.Visible;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            UpdateDGView();

            if (_curEditor == EmpEditor)
            {
                Employees emp = DataContext as Employees;

                if (string.IsNullOrWhiteSpace(emp.LastName) || string.IsNullOrWhiteSpace(emp.FirstName))
                    errors.AppendLine("Поля с фамилией и именем сотрудника не должны быть пустыми!");
                if (!(emp.Gender == "ж" || emp.Gender == "м"))
                    errors.AppendLine("Поле с полом сотрудника может принимать значения только \"ж\" или \"м\"!");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Проверьте корректность ввода данных");
                    return;
                }

                if (emp.Id == 0)
                    TestTaskEntities.GetContext().Employees.Add(emp);

                try
                {
                    TestTaskEntities.GetContext().SaveChanges();
                    EmpList1.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
                    EmpList2.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
                    DtGridEmp.ItemsSource = TestTaskEntities.GetContext().Employees.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Возникла ошибка добавления записи");
                }
            }
            else if (_curEditor == DepEditor)
            {
                Departments dep = DataContext as Departments;

                if (string.IsNullOrWhiteSpace(dep.Name))
                    errors.AppendLine("Название отдела не должно быть пустым!");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Проверьте корректность ввода данных");
                    return;
                }

                if (dep.Id == 0)
                    TestTaskEntities.GetContext().Departments.Add(dep);

                try
                {
                    TestTaskEntities.GetContext().SaveChanges();
                    DtGridDep.ItemsSource = TestTaskEntities.GetContext().Departments.ToList();
                    DepList.ItemsSource = TestTaskEntities.GetContext().Departments.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Возникла ошибка добавления записи");
                }
            }
            else
            {
                Orders ord = DataContext as Orders;

                if (string.IsNullOrWhiteSpace(ord.Counterparty))
                    errors.AppendLine("Название контрагента не должно быть пустым!");

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Проверьте корректность ввода данных");
                    return;
                }

                if (ord.Id == 0)
                    TestTaskEntities.GetContext().Orders.Add(ord);

                try
                {
                    TestTaskEntities.GetContext().SaveChanges();
                    DtGridOrd.ItemsSource = TestTaskEntities.GetContext().Orders.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Возникла ошибка добавления записи");
                }
            }

            HideEditor();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _curEditor.Children.OfType<TextBox>()
            .ToList().ForEach(x => x.Text = String.Empty);

            _curEditor.Children.OfType<ComboBox>()
            .ToList().ForEach(x => x.SelectedIndex = -1);

            _curEditor.Children.OfType<DatePicker>()
            .ToList().ForEach(x => x.SelectedDate = null);
        }

        private void UpdateDGView()
        {
            _curEditor.Children.OfType<TextBox>()
            .ToList().ForEach(x => x.GetBindingExpression(TextBox.TextProperty).UpdateSource());

            _curEditor.Children.OfType<ComboBox>()
            .ToList().ForEach(x => x.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource());

            _curEditor.Children.OfType<DatePicker>()
            .ToList().ForEach(x => x.GetBindingExpression(DatePicker.SelectedDateProperty).UpdateSource());
        }
    }
}
