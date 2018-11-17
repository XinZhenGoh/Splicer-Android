﻿// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if UNITY_ANDROID

using UnityEngine;

using GoogleMobileAds.Api;

namespace GoogleMobileAds.Android
{
    internal class InAppPurchaseResult : IInAppPurchaseResult
    {
        private AndroidJavaObject result;
        public InAppPurchaseResult(AndroidJavaObject result)
        {
            this.result = result;
        }

        public void FinishPurchase() {
            result.Call("finishPurchase");
        }

        public string ProductId {
            get { return result.Call<string>("getProductId"); }
        }

        public bool IsSuccessful {
            get {
                AndroidJavaObject pluginUtils = new AndroidJavaObject(Utils.PluginUtilsClassName);
                return pluginUtils.CallStatic<bool>("isResultSuccess", result);
            }
        }

        public bool IsVerified {
            get { return result.Call<bool>("isVerified"); }
        }
    }
}

#endif
