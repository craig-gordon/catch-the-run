'use strict';

const DynamoDB = require('aws-sdk/clients/dynamodb');

const dynamoClient = new DynamoDB.DocumentClient();

module.exports.processEvent = async (event, context) => {
    console.log('Lambda Context:', context);
    console.log('Create Feed Request:', event);

    const params = {
        TableName: 'Main',
        KeyConditionExpression: 'PRT = :PRT and begins_with (SRT, :SRT)',
        ExpressionAttributeValues: {
            ':PRT': event.ProviderName,
            ':SRT': 'F|SUB'
        }
    }

    try {
        const getAllSubsResponse = await dynamoClient.query(params).promise();
        console.log('all subs:', getAllSubsResponse);
        return {
            statusCode: 200,
            headers: {
                'Access-Control-Allow-Origin': '*'
            }
        }
    } catch (e) {
        return {
            statusCode: 200,
            headers: {
                'Access-Control-Allow-Origin': '*'
            }
        }
    }
}