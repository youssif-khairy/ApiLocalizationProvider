
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
        public long Id { get; set; }
        /// <summary>
        /// Translation key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// TranslationEnglish 
        /// </summary>
        public string TranslationEnglish { get; set; }
        /// <summary>
        /// TranslationArabic 
        /// </summary>
        public string TranslationArabic { get; set; }
        /// <summary>
        /// Is Translation Used For Frontend Else For Backend
        /// </summary>
        public bool IsFrontendTranslation { get; set; }
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
        public DateTime CreationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// LastUpdatedDate
        /// </summary>
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;


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
            TranslationArabic = clone.TranslationArabic;
            TranslationEnglish = clone.TranslationEnglish;
            IsFrontendTranslation = clone.IsFrontendTranslation;
            ResourceName = clone.ResourceName;
            IsDeleted = clone.IsDeleted;
            
        }
        
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="mutatedlocalization"></param>
        public LocalizationDetails(LocalizationMutatedDetailsDto mutatedlocalization)
        {
            Key = mutatedlocalization.Key;
            TranslationArabic = string.IsNullOrEmpty(mutatedlocalization.TranslationArabic) ? mutatedlocalization.OriginalTranslationArabic : mutatedlocalization.TranslationArabic;
            TranslationEnglish = string.IsNullOrEmpty(mutatedlocalization.TranslationEnglish) ? mutatedlocalization.OriginalTranslationEnglish: mutatedlocalization.TranslationEnglish;
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
        public void UpdateTranslation(string translationEnglish, string translationArabic)
        {
            TranslationArabic = translationArabic;
            TranslationEnglish = translationEnglish;
            LastUpdatedDate = DateTime.Now;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deletionDate"></param>
        public virtual void SoftDelete(DateTime deletionDate)
        {
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
            TranslationArabic = string.IsNullOrEmpty(mutatedlocalization.TranslationArabic) ? mutatedlocalization.OriginalTranslationArabic : mutatedlocalization.TranslationArabic;
            TranslationEnglish = string.IsNullOrEmpty(mutatedlocalization.TranslationEnglish) ? mutatedlocalization.OriginalTranslationEnglish : mutatedlocalization.TranslationEnglish;
            ResourceName = mutatedlocalization.ResourceName;    
            IsFrontendTranslation = mutatedlocalization.IsFrontendTranslation;
            IsDeleted = mutatedlocalization.IsDeleted;
            LastUpdatedDate = DateTime.Now;
        }

        #endregion
    }
}
