"use strict";

const DynamoDB = require("aws-sdk/clients/dynamodb");
const dynamoClient = new DynamoDB.DocumentClient();
const axios = require("axios").default;

const webPushService = require("web-push");

webPushService.setVapidDetails(
  "https://catchthe.run/",
  "BJMQ-CpMM_-OoO2hNPt3oM_pM8TSpPhSL_rGwGix99iLj0hdyHCZTP3XYeOO8sf9ghhHHFO2I7QpDrgSyGd1VQ4",
  "S_3sQfZvYAh7LGuKkXreV_kfcmAkdNFL0nAoI2z1P3w"
);

module.exports.processEvent = async (event, context) => {
  console.log("Lambda Context:", context);
  console.log("Create Feed Request:", event);

  const params = {
    TableName: "Main",
    KeyConditionExpression: "PRT = :PRT and begins_with (SRT, :SRT)",
    ExpressionAttributeValues: {
      ":PRT": event.ProviderName,
      ":SRT": "F|SUB"
    }
  };

  try {
    const getAllSubsResponse = await dynamoClient.query(params).promise();
    console.log("all subs:", getAllSubsResponse);

    const subs = getAllSubsResponse.Items;

    for (let i = 0; i < subs.length; i++) {
      const sub = subs[i];

      if (sub["WEBPUSH"]) {
        webPushService
          .sendNotification(JSON.parse(sub.WEBPUSH.Endpoint), event.Message)
          .then(res => console.log("webpush res:", res))
          .catch(err => console.log("webpush error:", err));
      }
      if (sub["DISCORD-AT"]) {
        axios.post(
          `https://discordapp.com/api/channels/${sub["DISCORD-AT"].DiscordServerId}/messages`,
          {
            content: "@" + sub["DISCORD-AT"].Endpoint,
            embed: {
              description: event.Message,
              color: 15155743
            }
          },
          {
            headers: {
              Authorization:
                "Bot NjM1NTc3MjkxNzQ4OTMzNzAz.XazGIQ.O4R5GiMXZNHLAs_ClPhOlIR99Zc",
              "Content-Type": "application/json"
            }
          }
        );
      }
    }

    // return {
    //   statusCode: 200,
    //   headers: {
    //     "Access-Control-Allow-Origin": "*"
    //   }
    // };
  } catch (e) {
    // return {
    //   statusCode: 200,
    //   headers: {
    //     "Access-Control-Allow-Origin": "*"
    //   }
    // };
  }
};
