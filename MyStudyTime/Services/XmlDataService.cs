using MyStudyTime.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace MyStudyTime.Services
{
    public class XmlDataService : IDataService
    {
        private readonly string _dataDirectory;
        private readonly string _subjectsFilePath;
        private readonly string _flashCardsFilePath;
        private readonly string _studyGoalsFilePath;

        public XmlDataService()
        {
            // Create data directory in AppData
            _dataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MyStudyTime"
            );

            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
            }

            _subjectsFilePath = Path.Combine(_dataDirectory, "subjects.xml");
            _flashCardsFilePath = Path.Combine(_dataDirectory, "flashcards.xml");
            _studyGoalsFilePath = Path.Combine(_dataDirectory, "studygoals.xml");
        }

        // ==================== Subjects ====================
        public ObservableCollection<Subject> LoadSubjects()
        {
            var subjects = new ObservableCollection<Subject>();

            try
            {
                if (File.Exists(_subjectsFilePath))
                {
                    var serializer = new XmlSerializer(typeof(SubjectsContainer));
                    using (var reader = new StreamReader(_subjectsFilePath))
                    {
                        var container = serializer.Deserialize(reader) as SubjectsContainer;
                        if (container?.Subjects != null)
                        {
                            foreach (var subject in container.Subjects)
                            {
                                subjects.Add(subject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subjects: {ex.Message}");
            }

            return subjects;
        }

        public void SaveSubjects(ObservableCollection<Subject> subjects)
        {
            try
            {
                var container = new SubjectsContainer { Subjects = new System.Collections.Generic.List<Subject>(subjects) };
                var serializer = new XmlSerializer(typeof(SubjectsContainer));

                using (var writer = new StreamWriter(_subjectsFilePath))
                {
                    serializer.Serialize(writer, container);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving subjects: {ex.Message}");
            }
        }

        // ==================== FlashCards ====================
        public ObservableCollection<FlashCard> LoadFlashCards()
        {
            var flashCards = new ObservableCollection<FlashCard>();

            try
            {
                if (File.Exists(_flashCardsFilePath))
                {
                    var serializer = new XmlSerializer(typeof(FlashCardsContainer));
                    using (var reader = new StreamReader(_flashCardsFilePath))
                    {
                        var container = serializer.Deserialize(reader) as FlashCardsContainer;
                        if (container?.FlashCards != null)
                        {
                            foreach (var card in container.FlashCards)
                            {
                                flashCards.Add(card);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading flash cards: {ex.Message}");
            }

            return flashCards;
        }

        public void SaveFlashCards(ObservableCollection<FlashCard> flashCards)
        {
            try
            {
                var container = new FlashCardsContainer { FlashCards = new System.Collections.Generic.List<FlashCard>(flashCards) };
                var serializer = new XmlSerializer(typeof(FlashCardsContainer));

                using (var writer = new StreamWriter(_flashCardsFilePath))
                {
                    serializer.Serialize(writer, container);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving flash cards: {ex.Message}");
            }
        }

        // ==================== StudyGoals ====================
        public ObservableCollection<StudyGoal> LoadStudyGoals()
        {
            var goals = new ObservableCollection<StudyGoal>();

            try
            {
                if (File.Exists(_studyGoalsFilePath))
                {
                    var serializer = new XmlSerializer(typeof(StudyGoalsContainer));
                    using (var reader = new StreamReader(_studyGoalsFilePath))
                    {
                        var container = serializer.Deserialize(reader) as StudyGoalsContainer;
                        if (container?.StudyGoals != null)
                        {
                            foreach (var goal in container.StudyGoals)
                            {
                                goals.Add(goal);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading study goals: {ex.Message}");
            }

            return goals;
        }

        public void SaveStudyGoals(ObservableCollection<StudyGoal> goals)
        {
            try
            {
                var container = new StudyGoalsContainer { StudyGoals = new System.Collections.Generic.List<StudyGoal>(goals) };
                var serializer = new XmlSerializer(typeof(StudyGoalsContainer));

                using (var writer = new StreamWriter(_studyGoalsFilePath))
                {
                    serializer.Serialize(writer, container);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving study goals: {ex.Message}");
            }
        }
    }

    // Container classes for XML serialization
    [XmlRoot("Subjects")]
    public class SubjectsContainer
    {
        [XmlElement("Subject")]
        public System.Collections.Generic.List<Subject> Subjects { get; set; }
    }

    [XmlRoot("FlashCards")]
    public class FlashCardsContainer
    {
        [XmlElement("FlashCard")]
        public System.Collections.Generic.List<FlashCard> FlashCards { get; set; }
    }

    [XmlRoot("StudyGoals")]
    public class StudyGoalsContainer
    {
        [XmlElement("StudyGoal")]
        public System.Collections.Generic.List<StudyGoal> StudyGoals { get; set; }
    }
}
