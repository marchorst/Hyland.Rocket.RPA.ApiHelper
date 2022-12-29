namespace Hyland.Rocket.RPA.ApiHelper.Routes
{
    using System;

    public class TaskWithDiversityAlreadyExistsException : Exception
    {
        /// <summary>
        /// TaskWithDiversityAlreadyExistsException
        /// </summary>
        /// <param name="message"></param>
        public TaskWithDiversityAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
