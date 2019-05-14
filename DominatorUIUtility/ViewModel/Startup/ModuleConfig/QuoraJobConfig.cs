
using System.Collections.Generic;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    class QuoraJobConfig : INetworkJobConfig
    {
        public Dictionary<string, object> RegisterJobConfigurations { get; set; } = new Dictionary<string, object>();
       
        public void RegisterJobConfiguration()
        {
            try
            {
                //RegisterJobConfigurations.Add(ActivityType.Follow.ToString(), new FollowerViewModel());
                //RegisterJobConfigurations.Add(ActivityType.Unfollow.ToString(), new UnfollowerViewModel());
                //RegisterJobConfigurations.Add(ActivityType.ReportAnswers.ToString(), new ReportAnswerViewModel());
                //RegisterJobConfigurations.Add(ActivityType.ReportUsers.ToString(), new ReportUserViewModel());
                //RegisterJobConfigurations.Add(ActivityType.AutoReplyToNewMessage.ToString(), new AutoReplyToNewMessageViewModel());
                //RegisterJobConfigurations.Add(ActivityType.SendMessageToFollower.ToString(), new SendMessageToFollowerViewModel());
                //RegisterJobConfigurations.Add(ActivityType.BroadcastMessages.ToString(), new BroadcastMessagesViewModel());
                //RegisterJobConfigurations.Add(ActivityType.UserScraper.ToString(), new UserScraperViewModel());
                //RegisterJobConfigurations.Add(ActivityType.AnswersScraper.ToString(), new AnswerScraperViewModel());
                //RegisterJobConfigurations.Add(ActivityType.QuestionsScraper.ToString(), new QuestionScraperViewModel());
                //RegisterJobConfigurations.Add(ActivityType.UpvoteAnswers.ToString(), new UpvoteAnswersViewModel());
                //RegisterJobConfigurations.Add(ActivityType.DownvoteQuestions.ToString(), new DownvoteQuestionsViewModel());
                //RegisterJobConfigurations.Add(ActivityType.DownvoteAnswers.ToString(), new DownvoteAnswersViewModel());
                //RegisterJobConfigurations.Add(ActivityType.AnswerOnQuestions.ToString(), new AnswerQuestionViewModel());
            }
            catch (System.Exception ex)
            {

            }
        }
    }

  
}
