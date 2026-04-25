using MyStudyTime.MVVM.Model;
using System.Collections.ObjectModel;

namespace MyStudyTime.Services
{
    public interface IDataService
    {
        // Subject operations
        ObservableCollection<Subject> LoadSubjects();
        void SaveSubjects(ObservableCollection<Subject> subjects);

        // FlashCard operations (global)
        ObservableCollection<FlashCard> LoadFlashCards();
        void SaveFlashCards(ObservableCollection<FlashCard> flashCards);

        // StudyGoal operations
        ObservableCollection<StudyGoal> LoadStudyGoals();
        void SaveStudyGoals(ObservableCollection<StudyGoal> goals);
    }
}
