using System;
using System.ComponentModel;
using System.Reflection;

namespace DominatorHouseCore.Utility
{
    [Localizable(false)]
    public static class Constants
    {
      
        public static string ApiUrl => $"{(object)INSTAGRAM_BASE_URL}api/v1/";

        public static string ApplicationName => "Socinator";

        public static string ContentTypeDefault => "application/x-www-form-urlencoded; charset=UTF-8";

      
     
        public static int FloodWait => 60000;

        public static string IG_SIG_KEY
        {
            get
            {
                // return "9cf2daac9a7802e9d199ad5dc6e221fe8da535359b58b60a1265a281b2a30a83";//64 version
                return "446f6292f1da63db9d8d3a9f5af793625173f79bb61de1ddd5cf10ef933a7af7";//94 version
            }
        }

        public static string IG_VERSION
        {
            get
            {
                return "40.0.0.14.95";
            }
        }

        public static string INSTAGRAM_BASE_URL
        {
            get
            {
                return "https://i.instagram.com/";
            }
        }

     
        public static string SIG_KEY_VERSION
        {
            get
            {
                return "4";
            }
        }

        public static string STATUS_OK_RESPONSE
        {
            get
            {
                return "{\"status\": \"ok\"}";
            }
        }

     

      
      
        public static string USERAGENT_FORMAT
        {
            get
            {
                return "Instagram {0} Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})";
            }
        }

        public static string USERAGENT_LOCALE
        {
            get
            {
                return "en_US";
            }
        }

     

        public static string X_FB_HTTP_Engine
        {
            get
            {
                return "Liger";
            }
        }

        public static string X_IG_Capabilities
        {
            get
            {
                return "3boDAA==";
            }
        }

        public static string X_IG_Connection_Type
        {
            get
            {
                return "WIFI";
            }
        }

        public static string Referer
        {
            get
            {
                return "https://i.instagram.com";
            }
        }


        public static string Origin
        {
            get
            {
                return "https://i.instagram.com";
            }
        }

        public static string USER_AGENT_LOCALE
        {
            get
            {
                return "en_US";
            }
        }


        public static string ACCEPT_LANGUAGE
        {
            get
            {
                return "en_US";
            }
        }

        //

        public static string ACCEPT_ENCODING
        {
            get
            {
                return "gzip,deflate";
            }
        }

        

        public static int AppVersion
        {
            get
            {
                try
                {
                    string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    return Int32.Parse(version.Substring(version.LastIndexOf(".") + 1));
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
                return 0;
            }
        }

        public static string ProductName = "Socinator";
        public static string ClickOnceFileName = "Socinator.appref-ms";

        public static string EXPERIMENTS = "ig_growth_android_profile_pic_prefill_with_fb_pic_2,ig_android_background_voice_phone_confirmation_prefilled_phone_number_only,ig_android_make_sure_next_button_is_visible_in_reg,ig_android_report_nux_completed_device,ig_android_background_voice_confirmation_block_argentinian_numbers,ig_android_device_verification_fb_signup,ig_android_reg_nux_headers_cleanup_universe,ig_android_reg_omnibox,ig_android_background_voice_phone_confirmation,ig_android_gmail_autocomplete_account_over_one_tap,ig_android_skip_signup_from_one_tap_if_no_fb_sso,ig_android_multi_tap_login,ig_android_access_flow_prefill,ig_android_contact_import_placement_universe,ig_android_run_device_verification,ig_android_onboarding_skip_fb_connect,ig_restore_focus_on_reg_textbox_universe,ig_android_phoneid_sync_interval,ig_android_security_intent_switchoff,ig_client_logging_efficiency,ig_android_show_password_in_reg_universe,ig_android_fci_onboarding_friend_search,ig_android_ui_cleanup_in_reg_v2,ig_android_editable_username_in_reg,ig_android_phone_auto_login_during_reg,ig_android_one_tap_fallback_auto_login,ig_android_updated_copy_user_lookup_failed,ig_android_hsite_prefill_new_carrier,ig_android_me_profile_prefill_in_reg,ig_android_allow_phone_reg_selectable,ig_android_gmail_oauth_in_reg,ig_android_run_account_nux_on_server_cue_device,ig_android_passwordless_auth,ig_android_sim_info_upload,ig_android_background_phone_confirmation_v2,ig_android_email_one_tap_auto_login_during_reg,ig_android_password_toggle_on_login_universe_v2,ig_android_refresh_onetap_nonce,ig_challenge_kill_switch,ig_android_modularized_nux_universe_device,ig_android_account_recovery_auto_login,ig_android_onetaplogin_login_upsell,ig_android_access_redesign_v3,ig_android_abandoned_reg_flow,ig_android_smartlock_hints_universe,ig_android_2fac_auto_fill_sms_universe,ig_android_onetaplogin_optimization,ig_android_family_apps_user_values_provider_universe,ig_android_direct_inbox_account_switching,ig_android_login_bad_password_autologin_universe,ig_android_account_switch_optimization,ig_android_rtl_password_hint,ig_android_device_sms_retriever_plugin_universe,ig_android_device_verification_separate_endpoint";

        public static string LoginConfig = "ig_android_dogfooding,ig_android_direct_gifs_killswitch,ig_android_mi_remove_netego_long_event,ig_android_newsfeed_recyclerview,ig_direct_e2e_send_waterfall_sample_rate_config,ig_android_insights_top_account_dialog_tooltip,ig_android_feed_report_ranking_issue,ig_android_shopping_django_product_search,ig_android_qp_waterfall_logging,ig_android_bug_report_screen_record,ig_android_stories_send_preloaded_reels_with_reels_tray,ig_android_qp_keep_promotion_during_cooldown,ig_launcher_ig_android_reactnative_realtime_ota,ig_android_employee_options,ig_android_qp_xshare_to_fb,ig_android_mi_block_expired_events,ig_android_request_compression_launcher,ig_story_insights_entry,ig_android_feed_attach_report_logs,ig_android_insights_welcome_dialog_tooltip,ig_android_react_native_ota_kill_switch,ig_android_notification_setting_sync";



    }
}
