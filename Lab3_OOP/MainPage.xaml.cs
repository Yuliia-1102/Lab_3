using System.Collections.ObjectModel;
using System.Reflection;

namespace Lab3_OOP
{
    public partial class MainPage : ContentPage
    {
        private string _filePath = "";
        private string _resultsFilePath = "";
        private bool _isError = false;
        private Dictionary<string, List<string>> _pickersData = new Dictionary<string, List<string>>
        {
            { "AuthorName", new List<string>() },
            { "Faculty", new List<string>() },
            { "Department", new List<string>() }
        };
        private ObservableCollection<ScienceWorker> _deserializedData = new ObservableCollection<ScienceWorker>();
        private ObservableCollection<ScienceWorker> _dataToShow = new ObservableCollection<ScienceWorker>();
        private Linq _analizator = new Linq();
        SearchCriteria criteria = new SearchCriteria();

        public MainPage()
        {
            InitializeComponent();
            ResultsListView.ItemsSource = _dataToShow;
        }

        private void AddPickerValue(ScienceWorker worker)
        {
            string[] selectedProperties = { "AuthorName", "Faculty", "Department" };

            foreach (string propertyName in selectedProperties)
            {
                PropertyInfo property = worker.GetType().GetProperty(propertyName);
                var pickerList = _pickersData[propertyName];

                if (property != null)
                {
                    string propertyValue = property.GetValue(worker) as string;
                    if (!string.IsNullOrEmpty(propertyValue) && !pickerList.Contains(propertyValue))
                    {
                       pickerList.Add(propertyValue);
                    }
                }
            }
        }

        private void ClearCriterias()
        {
            foreach (var list in _pickersData.Values)
            {
                list.Clear();
            }
        }

        private void ClearPickersValues()
        {
            authorPicker.ItemsSource = null;
            facultyPicker.ItemsSource = null;
            departmentPicker.ItemsSource = null;
        }

        private void SortPickersValues()
        {
            foreach (var list in _pickersData.Values)
            {
                list.Sort();
            }
        }

        private void AddItemSourses()
        {
            SortPickersValues();
            authorPicker.ItemsSource = _pickersData["AuthorName"];
            facultyPicker.ItemsSource = _pickersData["Faculty"];
            departmentPicker.ItemsSource = _pickersData["Department"];
        }

        private void FillPickers()
        {
            ClearCriterias();
           
            foreach(var worker in _deserializedData)
            {
                AddPickerValue(worker);
            }

            ClearPickersValues();
            AddItemSourses();
        }

        private void UpdateFilters()
        {
            authorPicker.SelectedItem = null;
            facultyPicker.SelectedItem = null;
            departmentPicker.SelectedItem = null; 
            _dataToShow.Clear();
            notFoundLabel.IsVisible = false;
        }

        private async Task<string> PickFile() //отримання шляху до файлу
        {
            _isError = false;
            try
            {
                var result = await FilePicker.PickAsync();
                if (result != null)
                {
                    string resultPath = result.FullPath;

                    if (File.Exists(resultPath))
                    {
                        string extension = Path.GetExtension(resultPath);
                        if (extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                        {
                            return resultPath;
                        }
                        else
                        {
                            _isError = true;
                            await DisplayAlert("Помилка", "Файл не є JSON-розширенням.", "ОК");
                        }
                    }
                    else
                    {
                        _isError = true;
                        await DisplayAlert("Помилка", "Обраного файлу не існує.", "ОК");
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _isError = true;
                await DisplayAlert("Помилка", $"{ex.Message}", "ОК");
                return string.Empty;
            }
        }

        private async void OnPickFileClicked(object sender, EventArgs e)
        {
            _filePath = await PickFile();

            if (!string.IsNullOrEmpty(_filePath) && !_isError)
            {
                FileInfo fileInfo = new FileInfo(_filePath);

                if(fileInfo.Length > 0)
                {
                    try
                    {
                        _deserializedData = JsonFunctions.Deserialize(_filePath);
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Помилка", ex.Message, "ОК");
                    }

                    UpdateFilters();
                    FillPickers();
                    filters.IsVisible = true;
                }
                else
                {
                    await DisplayAlert("Помилка", "Файл пустий.", "ОК");
                }
            }
        }

        private async void SaveJsonBtnClicked(object sender, EventArgs e)
        {
            _resultsFilePath = await PickFile();

            if (!string.IsNullOrEmpty(_resultsFilePath) && !_isError)
            {
                try
                {
                    JsonFunctions.Serialize(_resultsFilePath, _dataToShow);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Помилка", ex.Message, "ОК");
                }
                
                await DisplayAlert("Інформація", "Cеріалізація відбулась успішно.", "Оkay");
            }
        }

        private void OnCleanBtnClicked(object sender, EventArgs e)
        {
            UpdateFilters();
        }

        private SearchCriteria FormCriteria()
        {
            SearchCriteria newCriteria = new SearchCriteria();

            newCriteria.AuthorName = authorPicker.SelectedItem != null ? authorPicker.SelectedItem as string : string.Empty;
            newCriteria.Faculty = facultyPicker.SelectedItem != null ? facultyPicker.SelectedItem as string : string.Empty;
            newCriteria.Department = departmentPicker.SelectedItem != null ? departmentPicker.SelectedItem as string : string.Empty;
            return newCriteria;
        }

        private async void OnSearchBtnClicked(object sender, EventArgs e)
        {

            criteria = FormCriteria();

            try
            {
                _analizator.Search(criteria, _deserializedData, _dataToShow);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Помилка", $"{ex.Message}", "ОК");
            }

            if (_dataToShow.Count > 0 && !string.IsNullOrEmpty(_filePath))
            {
                ResultsContainer.IsVisible = true;
                notFoundLabel.IsVisible = false;
            }
            else
            {
                ResultsContainer.IsVisible = false;
                if (!string.IsNullOrEmpty(_filePath))
                {
                    notFoundLabel.IsVisible = true;
                }
            }

        }

        async private void OnHelpBtnClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Про програму",
                "Автор: Сірак Юлія Олександрівна, Курс: 2, Група: К-27, Рік виконання: 2023.\n" +
                "Програма вміє: серіалізувати/десеріалізувати файл з розширенням .json; знайти вміст файлу за певними фільтрами; " +
                "додавати, редагувати (через окрему форму) та видаляти фільтри; " +
                "зберегти результати пошуку в JSON-форматі (до окремого файлу).",
                "Оkay");
        }

        private void DeleteButtonClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ScienceWorker workToDel = (ScienceWorker)button.BindingContext;

            _dataToShow.Remove(workToDel);
            _deserializedData.Remove(workToDel);

            FillPickers();

            try
            {
                JsonFunctions.Serialize(_filePath, _deserializedData);
            }
            catch (Exception ex)
            {
                DisplayAlert("Помилка", ex.Message, "ОК");
            }
            
        }

        private async void OnChangeBtnClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ScienceWorker selectedItem = (ScienceWorker)button.BindingContext;
            if(selectedItem != null)
            {
                var secondPage = new EditPage(selectedItem);
                secondPage.DataModified += (s, modifiedData) =>
                {
                    if (modifiedData != null)
                    {
                        ResultsListView.ItemsSource = null;

                        int idxInShowData = _dataToShow.IndexOf(selectedItem);
                        int idxInAllData = _dataToShow.IndexOf(selectedItem);

                        _dataToShow[idxInShowData] = modifiedData;
                        _deserializedData[idxInAllData] = modifiedData;

                        ResultsListView.ItemsSource = _dataToShow;
                        DisplayAlert("Інформація", "Зміни зроблено.", "ОК");
                        FillPickers();

                        try
                        {
                            JsonFunctions.Serialize(_filePath, _deserializedData);
                        }
                        catch (Exception ex)
                        {
                            DisplayAlert("Помилка", ex.Message, "ОК");
                        }
                    }
                    else
                    {
                        DisplayAlert("Помилка", "Внести зміни не вдалося.", "ОК");
                    }
                };

                await Navigation.PushModalAsync(secondPage);
            }
            else
            {
                await DisplayAlert("Помилка", "Помилка.", "ОК");
            }
        }

        private async void OnAddElemBtnClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ScienceWorker newWork = new ScienceWorker();

            var secondPage = new EditPage(newWork);
            secondPage.DataModified += (s, modifiedData) =>
            {
                if (modifiedData != null)
                {
                    _deserializedData.Insert(0, modifiedData);
                    _analizator.Search(criteria, _deserializedData, _dataToShow);

                    DisplayAlert("Інфо", "Наукового працівника додано.", "ОК");
                    FillPickers();

                    try
                    {
                        JsonFunctions.Serialize(_filePath, _deserializedData);
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Помилка", ex.Message, "ОК");
                    }
                }
                else
                {
                    DisplayAlert("Помилка", "Внести зміни не вдалося.", "ОК");
                }
            };

            await Navigation.PushModalAsync(secondPage);
        }
    }
}