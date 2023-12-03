using System.Collections.ObjectModel;

namespace Lab3_OOP
{
    public partial class EditPage : ContentPage
    {
        private ScienceWorker _selectedItem;
        public event EventHandler<ScienceWorker> DataModified;

        private void FillInputs()
        {
            nameInput.Text = _selectedItem.Name;
            authorNameInput.Text = _selectedItem.AuthorName;
            facultyInput.Text = _selectedItem.Faculty;
            departInput.Text = _selectedItem.Department;
            posInput.Text = _selectedItem.AuthorPosition;
            birthInput.Text = _selectedItem.DateOfBirth;
            genderInput.Text = _selectedItem.Gender;
            addressInput.Text = _selectedItem.Address;
            ageInput.Text = _selectedItem.Age;
            branchInput.Text = _selectedItem.Branch;
        }
        public EditPage(ScienceWorker selected)
        {
            InitializeComponent();

            _selectedItem = selected;
            FillInputs();
        }
        
        private bool IsEmpty(string value) 
        {
            return value == string.Empty;
        }

        private bool ValidateAll()
        {
            return (
                !IsEmpty(nameInput.Text) &&
                !IsEmpty(authorNameInput.Text)&&
                !IsEmpty(facultyInput.Text)&&
                !IsEmpty(departInput.Text)&&
                !IsEmpty(posInput.Text)&&
                !IsEmpty(birthInput.Text)&&
                !IsEmpty(genderInput.Text)&&
                !IsEmpty(addressInput.Text)&&
                !IsEmpty(ageInput.Text)&&
                !IsEmpty(branchInput.Text)
                );
        }

        private void UpdateSelected()
        {
            _selectedItem.Name = nameInput.Text;
            _selectedItem.AuthorName = authorNameInput.Text;
            _selectedItem.Faculty = facultyInput.Text;
            _selectedItem.Department = departInput.Text;
            _selectedItem.AuthorPosition = posInput.Text;
            _selectedItem.DateOfBirth = birthInput.Text;
            _selectedItem.Gender = genderInput.Text;
            _selectedItem.Address = addressInput.Text;
            _selectedItem.Age = ageInput.Text;
            _selectedItem.Branch = branchInput.Text;
        }

        private void SaveButtonClicked(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                UpdateSelected();
                DataModified?.Invoke(this, _selectedItem);
                Navigation.PopModalAsync();
            }
            else
            {
                DisplayAlert("Помилка", "Перевірте введення.", "ОК");
            }
        }

        private void ReturnButtonClicked(object sender, EventArgs e)
        {
                Navigation.PopModalAsync();
        }
    }
}
