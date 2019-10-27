'use strict';

const DynamoDB = require('aws-sdk/clients/dynamodb');

const dynamoClient = new DynamoDB.DocumentClient();

module.exports.createSubscription = async (event, context) => {
    console.log('Lambda Context:', context);
    console.log('Create Subscription Request:', event);

    const params = {
        TableName: 'Main',
        Key: {
            PRT: event.ProviderName,
            SRT: `F|SUB|${event.ConsumerName}`
        }
    }

    let getSubResponse;

    try {
        getSubResponse = await dynamoClient.get(params).promise();
    } catch (e) {
        console.log('error:', e);
    }

    if (getSubResponse.Item) {
        const existingSub = getSubResponse.Item;
        existingSub[event.Protocol] = {
            Endpoint: event.Endpoint,
            DiscordServerId: event.DiscordServerId,
            FilterPolicy: event.FilterPolicy
        }

        try {
            const addSubInstanceResponse = await dynamoClient.put({ TableName: 'Main', Item: existingSub }).promise();
            return {
                statusCode: 200,
                headers: {
                    'Access-Control-Allow-Origin': '*'
                }
            }
        } catch (e) {
            console.log('error:', e);
        }
    } else {
        const newSub = {
            PRT: event.ProviderName,
            SRT: `F|SUB|${event.ConsumerName}`,
            G1S: event.ProviderName
        };
        newSub[event.Protocol] = {
            Endpoint: event.Endpoint,
            DiscordServerId: event.DiscordServerId,
            FilterPolicy: event.FilterPolicy
        }

        try {
            const addNewSubResponse = await dynamoClient.put({ TableName: 'Main', Item: newSub }).promise();
            return {
                statusCode: 200,
                headers: {
                    'Access-Control-Allow-Origin': '*'
                }
            }
        } catch (e) {
            console.log('error:', e);
        }
    }
}