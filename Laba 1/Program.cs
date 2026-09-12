using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Laboratory
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new LaboratoryForm());
    }
  }

  public class LaboratoryForm : Form
  {
    private TextBox lengthInput;
    private TextBox outerRadiusInput;
    private TextBox innerRadiusInput;
    private TextBox densityInput;
    private TextBox volumeOutput;
    private TextBox massOutput;
    private TextBox countInput;
    private TextBox cityInput;
    private TextBox peopleOutput;
    private TextBox averageAgeOutput;

    public LaboratoryForm()
    {
      Button calculateButton;
      Button generateButton;
      Button findButton;

      Text = "Laboratory 0 - Variant 4";
      ClientSize = new Size(700, 650);
      StartPosition = FormStartPosition.CenterScreen;

      createLabel("Task 1. Pipe volume and mass", 20, 20);

      createLabel("Length:", 20, 60);
      lengthInput = createTextBox(160, 57, 140);

      createLabel("Outer radius:", 20, 95);
      outerRadiusInput = createTextBox(160, 92, 140);

      createLabel("Inner radius:", 20, 130);
      innerRadiusInput = createTextBox(160, 127, 140);

      createLabel("Density:", 20, 165);
      densityInput = createTextBox(160, 162, 140);

      calculateButton = createButton("Calculate", 340, 55);
      calculateButton.Click += calculatePipeClick;

      createLabel("Volume:", 340, 110);
      volumeOutput = createTextBox(430, 107, 230);
      volumeOutput.ReadOnly = true;

      createLabel("Mass:", 340, 150);
      massOutput = createTextBox(430, 147, 230);
      massOutput.ReadOnly = true;

      createLabel("Task 2. People by city", 20, 230);

      createLabel("Record count:", 20, 275);
      countInput = createTextBox(160, 272, 140);
      countInput.Text = "10";

      generateButton = createButton("Generate CSV", 340, 268);
      generateButton.Click += generatePeopleClick;

      createLabel("City:", 20, 320);
      cityInput = createTextBox(160, 317, 140);
      cityInput.Text = "Kemerovo";

      findButton = createButton("Find people", 340, 313);
      findButton.Click += findPeopleClick;

      peopleOutput = createTextBox(20, 365, 640);
      peopleOutput.Multiline = true;
      peopleOutput.Height = 210;
      peopleOutput.ReadOnly = true;
      peopleOutput.ScrollBars = ScrollBars.Vertical;

      createLabel("Average age:", 20, 600);
      averageAgeOutput = createTextBox(160, 597, 140);
      averageAgeOutput.ReadOnly = true;
    }

    private void createLabel(string text, int left, int top)
    {
      Label label;

      label = new Label();
      label.Text = text;
      label.Left = left;
      label.Top = top;
      label.AutoSize = true;

      Controls.Add(label);
    }

    private TextBox createTextBox(int left, int top, int width)
    {
      TextBox textBox;

      textBox = new TextBox();
      textBox.Left = left;
      textBox.Top = top;
      textBox.Width = width;

      Controls.Add(textBox);

      return textBox;
    }

    private Button createButton(string text, int left, int top)
    {
      Button button;

      button = new Button();
      button.Text = text;
      button.Left = left;
      button.Top = top;
      button.Width = 160;
      button.Height = 32;

      Controls.Add(button);

      return button;
    }

    private void calculatePipeClick(object sender, EventArgs e)
    {
      double length;
      double outerRadius;
      double innerRadius;
      double density;
      double volume;
      double mass;

      volumeOutput.Clear();
      massOutput.Clear();

      if (!double.TryParse(lengthInput.Text, out length)
        || !double.TryParse(outerRadiusInput.Text, out outerRadius)
        || !double.TryParse(innerRadiusInput.Text, out innerRadius)
        || !double.TryParse(densityInput.Text, out density))
      {
        MessageBox.Show("Enter valid numbers.");
        return;
      }

      if (double.IsNaN(length) || double.IsInfinity(length)
        || double.IsNaN(outerRadius) || double.IsInfinity(outerRadius)
        || double.IsNaN(innerRadius) || double.IsInfinity(innerRadius)
        || double.IsNaN(density) || double.IsInfinity(density)
        || length <= 0.0 || innerRadius <= 0.0
        || outerRadius <= innerRadius || density <= 0.0)
      {
        MessageBox.Show(
          "Values must be positive and finite. " +
          "Outer radius must exceed inner radius."
        );
        return;
      }

      volume = Math.PI * length
        * (outerRadius * outerRadius - innerRadius * innerRadius);
      mass = density * volume;

      if (double.IsNaN(volume) || double.IsInfinity(volume)
        || double.IsNaN(mass) || double.IsInfinity(mass))
      {
        MessageBox.Show("The values are too large.");
        return;
      }

      volumeOutput.Text = volume.ToString("G10");
      massOutput.Text = mass.ToString("G10");
    }

    private void generatePeopleClick(object sender, EventArgs e)
    {
      string[] lastNames;
      string[] cities;
      Random random;
      int recordCount;
      int recordIndex;
      int lastNameIndex;
      int cityIndex;
      int minimumAge;
      int maximumAge;
      int age;

      if (!int.TryParse(countInput.Text, out recordCount)
        || recordCount <= 0)
      {
        MessageBox.Show("Enter a positive record count.");
        return;
      }

      lastNames = new string[] {
        "Smith", "Brown", "Wilson", "Taylor", "Clark"
      };
      cities = new string[] {
        "Kemerovo", "Tomsk", "Omsk", "Krasnoyarsk"
      };

      random = new Random();
      minimumAge = 1;
      maximumAge = 100;

      try
      {
        using (StreamWriter writer = new StreamWriter("people.csv"))
        {
          writer.WriteLine("lastName;city;age");

          for (recordIndex = 0; recordIndex < recordCount; recordIndex++)
          {
            lastNameIndex = random.Next(lastNames.Length);
            cityIndex = random.Next(cities.Length);
            age = random.Next(minimumAge, maximumAge + 1);

            writer.WriteLine(
              lastNames[lastNameIndex] + ";" +
              cities[cityIndex] + ";" +
              age
            );
          }
        }

        averageAgeOutput.Clear();
        peopleOutput.Text = "Data saved to people.csv";
      }
      catch (IOException)
      {
        MessageBox.Show("Could not write the file.");
      }
      catch (UnauthorizedAccessException)
      {
        MessageBox.Show("No permission to write the file.");
      }
    }

    private void findPeopleClick(object sender, EventArgs e)
    {
      string selectedCity;
      string line;
      string[] fields;
      int age;
      int personCount;
      double ageTotal;
      double averageAge;

      selectedCity = cityInput.Text.Trim();
      personCount = 0;
      ageTotal = 0.0;

      peopleOutput.Clear();
      averageAgeOutput.Clear();

      if (selectedCity.Length == 0)
      {
        MessageBox.Show("Enter a city.");
        return;
      }

      if (!File.Exists("people.csv"))
      {
        MessageBox.Show("Generate the CSV file first.");
        return;
      }

      try
      {
        using (StreamReader reader = new StreamReader("people.csv"))
        {
          line = reader.ReadLine();

          if (line != "lastName;city;age")
          {
            MessageBox.Show("Invalid CSV header.");
            return;
          }

          line = reader.ReadLine();

          while (line != null)
          {
            fields = line.Split(';');

            if (fields.Length != 3)
            {
              peopleOutput.Clear();
              MessageBox.Show("Invalid CSV record.");
              return;
            }

            if (!int.TryParse(fields[2], out age) || age < 0)
            {
              peopleOutput.Clear();
              MessageBox.Show("Invalid age in the file.");
              return;
            }

            if (string.Equals(
              fields[1], selectedCity, StringComparison.OrdinalIgnoreCase
            ))
            {
              ageTotal = ageTotal + age;
              personCount++;

              peopleOutput.AppendText(
                fields[0] + " | " +
                fields[1] + " | Age: " +
                age + Environment.NewLine
              );
            }

            line = reader.ReadLine();
          }
        }

        if (personCount > 0)
        {
          averageAge = ageTotal / personCount;
          averageAgeOutput.Text = averageAge.ToString("F2");
        }
        else
        {
          peopleOutput.Text = "No people found.";
        }
      }
      catch (IOException)
      {
        peopleOutput.Clear();
        MessageBox.Show("Could not read the file.");
      }
      catch (UnauthorizedAccessException)
      {
        peopleOutput.Clear();
        MessageBox.Show("No permission to read the file.");
      }
    }
  }
}