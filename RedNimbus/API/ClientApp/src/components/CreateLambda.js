import React from 'react';
import { Modal, Button } from "react-bootstrap";
import axios from 'axios';

class CreateLambda extends React.Component {

    constructor(props) {
        super(props);

        this.state = {
            file: null
        }

        this.createLambda = this.createLambda.bind(this);

        this.onChange = this.onChange.bind(this);

    }

    createLambda(event) {

        event.preventDefault();

        const options = {
            headers: { 'token': localStorage.getItem("token") }
        };

        var lambdaName = document.getElementById("lambdaName").value;
        var runtime = document.getElementById("runtime").value;
        var trigger = document.getElementById("trigger").value;

        var pathL = this.props.path;

            let fileReader = new FileReader();

            fileReader.onload = (event) => {
                axios.post("http://localhost:65001/api/lambda/createLambda", { "File": event.target.result, "Path": this.props.path, "Value": this.state.file.name, "Name": lambdaName, "Runtime": runtime, "Trigger": trigger }, options).then(
                    (resp) => this.onSuccessHandler(resp),
                    (resp) => this.onErrorHandler(resp)
                );

            
            fileReader.readAsDataURL(this.state.file);
        }
    }


    onErrorHandler(resp) {
        alert(resp.response.data);
    }

    onSuccessHandler(resp) {
        this.props.addLambda(resp.data);
        this.props.onHide();
    }

    onChange(e) {
        e.preventDefault();
        this.setState({ file: e.target.files[0] })
    }


    render() {
        return (
            <div>
                <Button variant="primary" onClick={() => this.props.onHide(true)}>
                    Create Lambda
                </Button>
                <Modal
                    show={this.props.show}
                    size="lg"
                    aria-labelledby="contained-modal-title-vcenter"
                    centered
                >
                    <Modal.Header>
                        <Modal.Title id="contained-modal-title-vcenter">
                            New Lambda
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <form onSubmit={this.createLambda}>
                            <div className="form-group">
                                <input type="text"
                                    className="form-control form-control-sm"
                                    id="lambdaName"
                                    name="newLambda"
                                    placeholder="Enter name"
                                    required
                                />
                                <br/>
                                <select id="runtime" name="runtime" className="form-control form-control-sm" required>
                                    <option value=".NET Core">.NET Core</option>
                                </select>
                                <br />
                                <select id="trigger" name="trigger" className="form-control form-control-sm" required>
                                    <option value="GET">GET</option>
                                    <option value="POST">POST</option>
                                </select>
                                <br />
                                <div>
                                    <label>Select File</label>
                                    <input type="file" name="file" id="file" onChange={this.onChange} />
                                </div>
                            </div>
                            <hr/>
                            <Button type="submit">Create</Button>
                            <Button onClick={() => this.props.onHide(false)}>Close</Button>
                        </form>
                    </Modal.Body>
                </Modal>
            </div>
        );
    }

}

export default CreateLambda;