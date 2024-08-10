
using ApiLocalizationProvider.DTO;
using System;

namespace ApiLocalizationProvider.Entities
{
    /// <summary>
    /// LocalizationDetails
    /// </summary>
    public class LocalizationDetails
    {
        #region PROPS
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; protected set; }
        /// <summary>
        /// Translation key
        /// </summary>
        public string Key { get; protected set; }
        /// <summary>
        /// OriginalTranslationEnglish by developer 
        /// </summary>
        public string OriginalTranslationEnglish { get; protected set; }
        /// <summary>
        /// OriginalTranslationArabic by developer 
        /// </summary>
        public string OriginalTranslationArabic { get; protected set; }
        /// <summary>
        /// TranslationEnglish 
        /// </summary>
        public string TranslationEnglish { get; protected set; }
        /// <summary>
        /// TranslationArabic 
        /// </summary>
        public string TranslationArabic { get; protected set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; protected set; }
        /// <summary>
        /// Notes
        /// </summary>
        public string Notes { get; protected set; }
        /// <summary>
        /// Number of times this translation is used in module
        /// </summary>
        public int UsageCount { get; protected set; }
        /// <summary>
        /// compnents paths used in 
        /// </summary>
        public string UsagePaths { get;protected set; }
        /// <summary>
        /// Is Translation Used For Frontend Else For Backend
        /// </summary>
        public bool IsFrontendTranslation { get; protected set; }
        /// <summary>
        /// ResourceName
        /// </summary>
        public string ResourceName { get; set; }
        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; } = false;
        /// <summary>
        /// CreationDate
        /// </summary>
        public DateTime CreationDate { get; protected set; } = DateTime.Now;
        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; protected set; }
        /// <summary>
        /// LastUpdatedDate
        /// </summary>
        public DateTime LastUpdatedDate { get; protected set; } = DateTime.Now;
        /// <summary>
        /// LastUpdatedBy
        /// </summary>
        public string LastUpdatedBy { get; protected set; }

        #endregion

        #region CTOR
        /// <summary>
        /// CTOR
        /// </summary>
        public LocalizationDetails()
        {
            
        }
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="clone"></param>
        public LocalizationDetails(LocalizationDetails clone)
        {
            Id = clone.Id;
            Key = clone.Key;
            OriginalTranslationEnglish = clone.OriginalTranslationEnglish;
            OriginalTranslationArabic = clone.OriginalTranslationArabic;
            TranslationArabic = clone.TranslationArabic;
            TranslationEnglish = clone.TranslationEnglish;
            Description = clone.Description;
            Notes = clone.Notes;
            UsageCount = clone.UsageCount;
            UsagePaths = clone.UsagePaths;
            IsFrontendTranslation = clone.IsFrontendTranslation;
            ResourceName = clone.ResourceName;
            IsDeleted = clone.IsDeleted;
            
        }
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="localizationDetails"></param>
        public LocalizationDetails(LocalizationDetailsDto localizationDetails)
        {
            Key = localizationDetails.Key;
            OriginalTranslationEnglish = localizationDetails.TranslationEnglish;
            OriginalTranslationArabic = localizationDetails.TranslationArabic;
            Description = localizationDetails.Description;
            UsageCount = localizationDetails.UsageCount;
            UsagePaths = string.Join(",", localizationDetails.UsagePaths);
            IsFrontendTranslation = localizationDetails.IsFrontendTranslation;
            ResourceName = localizationDetails.ResourceName;
        }
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="mutatedlocalization"></param>
        public LocalizationDetails(LocalizationMutatedDetailsDto mutatedlocalization)
        {
            Key = mutatedlocalization.Key;
            OriginalTranslationArabic = mutatedlocalization.OriginalTranslationArabic;
            OriginalTranslationEnglish = mutatedlocalization.OriginalTranslationEnglish;
            TranslationArabic = mutatedlocalization.TranslationArabic;
            TranslationEnglish = mutatedlocalization.TranslationEnglish;
            Notes = mutatedlocalization.Notes;
            Description = mutatedlocalization.Description;
            UsageCount = mutatedlocalization.UsageCount;
            UsagePaths = mutatedlocalization.UsagePaths;
            ResourceName = mutatedlocalization.ResourceName;
            IsFrontendTranslation = mutatedlocalization.IsFrontendTranslation;
            IsDeleted = mutatedlocalization.IsDeleted;
            LastUpdatedDate = DateTime.Now;
        }
        #endregion

        #region Actions
        /// <summary>
        /// UpdateTranslation
        /// </summary>
        public void UpdateTranslation(string translationEnglish, string translationArabic, string notes,string userId)
        {
            TranslationArabic = translationArabic;
            TranslationEnglish = translationEnglish;
            Notes = notes;
            LastUpdatedDate = DateTime.Now;
            LastUpdatedBy = userId;
        }
        /// <summary>
        /// Update Translations from Topic
        /// </summary>
        /// <param name="localizationDetails"></param>
        public void UpdateTranslation(LocalizationDetailsDto localizationDetails)
        {
            OriginalTranslationEnglish = localizationDetails.TranslationEnglish;
            OriginalTranslationArabic = localizationDetails.TranslationArabic;
            Description = localizationDetails.Description;
            UsageCount = localizationDetails.UsageCount;
            UsagePaths = string.Join(",", localizationDetails.UsagePaths);
            LastUpdatedDate = DateTime.Now;
            LastUpdatedBy = localizationDetails.UserId;
            IsDeleted = localizationDetails.IsDeleted;
            ResourceName = localizationDetails.ResourceName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deletionDate"></param>
        public virtual void SoftDelete(string userId, DateTime deletionDate)
        {
            LastUpdatedBy = userId;
            LastUpdatedDate = deletionDate;
            IsDeleted = true;
        }

        /// <summary>
        /// LocalizationMutatedDetailsDto
        /// </summary>
        /// <param name="mutatedlocalization"></param>
        public void UpdateLocalization(LocalizationMutatedDetailsDto mutatedlocalization)
        {
            Key = mutatedlocalization.Key;
            OriginalTranslationArabic = mutatedlocalization.OriginalTranslationArabic;
            OriginalTranslationEnglish = mutatedlocalization.OriginalTranslationEnglish;
            TranslationArabic = mutatedlocalization.TranslationArabic;
            TranslationEnglish = mutatedlocalization.TranslationEnglish;
            Notes = mutatedlocalization.Notes;
            Description = mutatedlocalization.Description;
            UsageCount = mutatedlocalization.UsageCount;
            UsagePaths = mutatedlocalization.UsagePaths;
            ResourceName = mutatedlocalization.ResourceName;    
            IsFrontendTranslation = mutatedlocalization.IsFrontendTranslation;
            IsDeleted = mutatedlocalization.IsDeleted;
            LastUpdatedDate = DateTime.Now;
        }

        #endregion
    }
}
