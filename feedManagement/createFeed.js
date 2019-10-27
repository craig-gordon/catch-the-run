'use strict';

const DynamoDB = require('aws-sdk/clients/dynamodb');

const dynamoClient = new DynamoDB.DocumentClient();

module.exports.createFeed = (event, context) => {
    console.log('Lambda Context:', context);
    console.log('Create Feed Request:', event);

    const params = {
        TableName: 'Main',
        Item: {
            PRT: event.ProviderName,
            SRT: 'F',
            G1S: event.ProviderName
        }
    }

    try {
        const putItemResponse = dynamoClient.put(params).promise();
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