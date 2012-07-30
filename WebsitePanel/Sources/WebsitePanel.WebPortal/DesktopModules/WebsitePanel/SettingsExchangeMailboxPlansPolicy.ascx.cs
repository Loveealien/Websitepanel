﻿// Copyright (c) 2012, Outercurve Foundation.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must  retain  the  above copyright notice, this
//   list of conditions and the following disclaimer.
//
// - Redistributions in binary form  must  reproduce the  above  copyright  notice,
//   this list of conditions  and  the  following  disclaimer in  the documentation
//   and/or other materials provided with the distribution.
//
// - Neither  the  name  of  the  Outercurve Foundation  nor   the   names  of  its
//   contributors may be used to endorse or  promote  products  derived  from  this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING,  BUT  NOT  LIMITED TO, THE IMPLIED
// WARRANTIES  OF  MERCHANTABILITY   AND  FITNESS  FOR  A  PARTICULAR  PURPOSE  ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,  EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO,  PROCUREMENT  OF  SUBSTITUTE  GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)  HOWEVER  CAUSED AND ON
// ANY  THEORY  OF  LIABILITY,  WHETHER  IN  CONTRACT,  STRICT  LIABILITY,  OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE)  ARISING  IN  ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using WebsitePanel.EnterpriseServer;
using WebsitePanel.Providers.HostedSolution;

namespace WebsitePanel.Portal
{
    public partial class SettingsExchangeMailboxPlansPolicy : WebsitePanelControlBase, IUserSettingsEditorControl
    {

        internal static List<ExchangeMailboxPlan> list;

        public void BindSettings(UserSettings settings)
        {

            if (list == null)
                list = new List<ExchangeMailboxPlan>();

            if (!string.IsNullOrEmpty(settings[UserSettings.DEFAULT_MAILBOXPLANS]))
            {

                XmlSerializer serializer = new XmlSerializer(list.GetType());

                StringReader reader = new StringReader(settings[UserSettings.DEFAULT_MAILBOXPLANS]);

                list = (List<ExchangeMailboxPlan>)serializer.Deserialize(reader);
            }

            gvMailboxPlans.DataSource = list;
            gvMailboxPlans.DataBind();

            if (gvMailboxPlans.Rows.Count <= 1)
            {
                btnSetDefaultMailboxPlan.Enabled = false;
            }
            else
                btnSetDefaultMailboxPlan.Enabled = true;
        }





        public string IsChecked(bool val)
        {
            return val ? "checked" : "";
        }


        public void btnAddMailboxPlan_Click(object sender, EventArgs e)
        {
            int count = 0;
            if (list != null)
            {
                foreach (ExchangeMailboxPlan p in list)
                {
                    p.MailboxPlanId = count;
                    count++;
                }
            }


            ExchangeMailboxPlan plan = new ExchangeMailboxPlan();
            plan.MailboxPlan = txtMailboxPlan.Text;
            plan.MailboxSizeMB = mailboxSize.ValueKB;
            if ((plan.MailboxSizeMB == 0)) plan.MailboxSizeMB = 1;
            plan.MailboxPlanId = count;
            plan.IsDefault = false;
            plan.MaxRecipients = maxRecipients.ValueKB;
            plan.MaxSendMessageSizeKB = maxSendMessageSizeKB.ValueKB;
            plan.MaxReceiveMessageSizeKB = maxReceiveMessageSizeKB.ValueKB;
            plan.EnablePOP = chkPOP3.Checked;
            plan.EnableIMAP = chkIMAP.Checked;
            plan.EnableOWA = chkOWA.Checked;
            plan.EnableMAPI = chkMAPI.Checked;
            plan.EnableActiveSync = chkActiveSync.Checked;
            plan.IssueWarningPct = sizeIssueWarning.ValueKB;
            if ((plan.IssueWarningPct == 0)) plan.IssueWarningPct = 100;
            plan.ProhibitSendPct = sizeProhibitSend.ValueKB;
            if ((plan.ProhibitSendPct == 0)) plan.ProhibitSendPct = 100;
            plan.ProhibitSendReceivePct = sizeProhibitSendReceive.ValueKB;
            if ((plan.ProhibitSendReceivePct == 0)) plan.ProhibitSendReceivePct = 100;
            plan.KeepDeletedItemsDays = daysKeepDeletedItems.ValueDays;
            plan.HideFromAddressBook = chkHideFromAddressBook.Checked;

            if (list == null)
                list = new List<ExchangeMailboxPlan>();

            list.Add(plan);
            gvMailboxPlans.DataSource = list;
            gvMailboxPlans.DataBind();

            if (gvMailboxPlans.Rows.Count <= 1)
            {
                btnSetDefaultMailboxPlan.Enabled = false;
            }
            else
                btnSetDefaultMailboxPlan.Enabled = true;

        }

        protected void gvMailboxPlan_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int mailboxPlanId = Utils.ParseInt(e.CommandArgument.ToString(), 0);

            switch (e.CommandName)
            {
                case "DeleteItem":

                    foreach (ExchangeMailboxPlan p in list)
                    {
                        if (p.MailboxPlanId == mailboxPlanId)
                        {
                            list.Remove(p);
                            break;
                        }
                    }

                    gvMailboxPlans.DataSource = list;
                    gvMailboxPlans.DataBind();

                    if (gvMailboxPlans.Rows.Count <= 1)
                    {
                        btnSetDefaultMailboxPlan.Enabled = false;
                    }
                    else
                        btnSetDefaultMailboxPlan.Enabled = true;
                    break;

                case "EditItem":
                    foreach (ExchangeMailboxPlan p in list)
                    {
                        if (p.MailboxPlanId == mailboxPlanId)
                        {

                            txtMailboxPlan.Text = p.MailboxPlan;
                            mailboxSize.ValueKB = p.MailboxSizeMB;
                            maxRecipients.ValueKB = p.MaxRecipients;
                            maxSendMessageSizeKB.ValueKB = p.MaxSendMessageSizeKB;
                            maxReceiveMessageSizeKB.ValueKB = p.MaxReceiveMessageSizeKB;
                            chkPOP3.Checked = p.EnablePOP;
                            chkIMAP.Checked = p.EnableIMAP;
                            chkOWA.Checked = p.EnableOWA;
                            chkMAPI.Checked = p.EnableMAPI;
                            chkActiveSync.Checked = p.EnableActiveSync;
                            sizeIssueWarning.ValueKB = p.IssueWarningPct;
                            sizeProhibitSend.ValueKB = p.ProhibitSendPct;
                            sizeProhibitSendReceive.ValueKB = p.ProhibitSendReceivePct;
                            daysKeepDeletedItems.ValueDays = p.KeepDeletedItemsDays;
                            chkHideFromAddressBook.Checked = p.HideFromAddressBook;

                            break;
                        }
                    }

                    break;
            }
        }

        protected void btnSetDefaultMailboxPlan_Click(object sender, EventArgs e)
        {
            // get domain
            int mailboxPlanId = Utils.ParseInt(Request.Form["DefaultMailboxPlan"], 0);



            foreach (ExchangeMailboxPlan p in list)
            {
                p.IsDefault = false;
            }

            foreach (ExchangeMailboxPlan p in list)
            {
                if (p.MailboxPlanId == mailboxPlanId)
                {
                    p.IsDefault = true;
                    break;
                }
            }

            gvMailboxPlans.DataSource = list;
            gvMailboxPlans.DataBind();
        }

        public void SaveSettings(UserSettings settings)
        {
            XmlSerializer serializer = new XmlSerializer(list.GetType());

            StringWriter writer = new StringWriter();

            serializer.Serialize(writer, list);

            settings[UserSettings.DEFAULT_MAILBOXPLANS] = writer.ToString();
        }

    }
}